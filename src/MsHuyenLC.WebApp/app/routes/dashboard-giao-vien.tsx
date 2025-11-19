import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { getProfile, updateProfile } from "~/apis/Profile";
import { VaiTro, type Profile, type LichHoc } from "~/types/index";
import { getLichHocByTeacher } from "~/apis/LichHoc";
import { uploadAvatar } from "~/apis/Upload";
import { createThongBao } from "~/apis/ThongBao";
import { getLopHocStudents } from "~/apis/LopHoc";
import {
  User, Calendar, Clock, Mail, Phone, MapPin, Edit, X, Camera,
  Bell, Send, Users, GraduationCap, FileText
} from "lucide-react";
import { getGiaoVien } from "~/apis";

export default function DashboardGiaoVien() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [profile, setProfile] = useState<Profile>();
  const [lichHocs, setLichHocs] = useState<LichHoc[]>([]);
  const [showEditModal, setShowEditModal] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState("");
  const [editForm, setEditForm] = useState({
    email: "",
    sdt: "",
    avatar: ""
  });

  // Notification states
  const [showNotificationModal, setShowNotificationModal] = useState(false);
  const [notificationForm, setNotificationForm] = useState({
    title: "",
    message: ""
  });
  const [teacherClasses, setTeacherClasses] = useState<any[]>([]);
  const [classStudents, setClassStudents] = useState<any[]>([]);
  const [loadingClassStudents, setLoadingClassStudents] = useState(false);
  const [sendType, setSendType] = useState<'all' | 'class' | 'individual'>('all');
  const [selectedClassId, setSelectedClassId] = useState<string>("");
  const [selectedStudentIds, setSelectedStudentIds] = useState<string[]>([]);

  useEffect(() => {
    loadDashboardData();
  }, []);

  useEffect(() => {
    if (selectedClassId) {
      loadClassStudents(selectedClassId);
    } else {
      setClassStudents([]);
      setSelectedStudentIds([]);
    }
  }, [selectedClassId]);

  const loadDashboardData = async () => {
    setLoading(true);

    const profileRes = await getProfile();
    if (!profileRes.success || !profileRes.data) {
      navigate("/dang-nhap");
      return;
    }

    setProfile(profileRes.data);

    if (profileRes.data.vaiTro !== VaiTro.GiaoVien) {
      navigate("/dashboard");
      return;
    }
    const lichDayRes = await getLichHocByTeacher(profileRes.data.id!);
    if (lichDayRes.success && Array.isArray(lichDayRes.data)) {
      setLichHocs(lichDayRes.data);

      // Get unique classes
      const uniqueClasses = lichDayRes.data
        .filter((lich: LichHoc) => lich.lopHoc)
        .reduce((acc: any[], lich: LichHoc) => {
          if (lich.lopHoc && !acc.find(c => c.id === lich.lopHoc!.id)) {
            acc.push(lich.lopHoc);
          }
          return acc;
        }, []);
      setTeacherClasses(uniqueClasses);
    }

    setLoading(false);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return "—";
    const date = new Date(dateString);
    return date.toLocaleDateString("vi-VN");
  };

  const getDayOfWeekText = (thu?: number) => {
    const daysOfWeek = ["Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7"];
    return thu !== undefined ? daysOfWeek[thu] || "—" : "—";
  };

  const formatTime = (timeString?: string) => {
    if (!timeString) return "—";
    return timeString.substring(0, 5);
  };

  const handleEditProfile = () => {
    setEditForm({
      email: profile?.email || "",
      sdt: profile?.sdt || "",
      avatar: profile?.avatar || ""
    });
    setShowEditModal(true);
    setMessage("");
  };

  const handleAvatarUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      setMessage("Vui lòng chọn file ảnh");
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      setMessage("File ảnh không được vượt quá 5MB");
      return;
    }

    setUploading(true);
    const result = await uploadAvatar(file);
    setUploading(false);

    if (result.success && result.url) {
      setEditForm(prev => ({ ...prev, avatar: result.url! }));
      setMessage("Upload ảnh thành công");
    } else {
      setMessage(result.message || "Upload ảnh thất bại");
    }
  };

  const handleSaveProfile = async () => {
    setSaving(true);
    setMessage("");

    try {
      const profileUpdate = await updateProfile({
        email: editForm.email,
        sdt: editForm.sdt,
        avatar: editForm.avatar
      });

      if (!profileUpdate.success) {
        setMessage(profileUpdate.message || "Cập nhật thông tin thất bại");
        setSaving(false);
        return;
      }

      setMessage("Cập nhật thông tin thành công");
      setTimeout(() => {
        setShowEditModal(false);
        loadDashboardData();
      }, 1500);
    } catch (error) {
      setMessage("Có lỗi xảy ra khi cập nhật thông tin");
    } finally {
      setSaving(false);
    }
  };

  const loadClassStudents = async (classId: string) => {
    setLoadingClassStudents(true);
    try {
      const result = await getLopHocStudents(classId);
      if (result.success && result.data) {
        const danhSach = result.data.danhSachHocVien || [];
        setClassStudents(Array.isArray(danhSach) ? danhSach : []);
      } else {
        setClassStudents([]);
      }
    } catch (error) {
      console.error("Error loading students:", error);
      setClassStudents([]);
    } finally {
      setLoadingClassStudents(false);
    }
  };

  const toggleStudentSelection = (studentId: string) => {
    setSelectedStudentIds(prev => {
      if (prev.includes(studentId)) {
        return prev.filter(id => id !== studentId);
      } else {
        return [...prev, studentId];
      }
    });
  };

  const toggleAllStudents = () => {
    if (selectedStudentIds.length === classStudents.length) {
      setSelectedStudentIds([]);
    } else {
      setSelectedStudentIds(classStudents.map(s => s.taiKhoanId).filter(Boolean));
    }
  };

  const handleSendNotification = async () => {
    if (!notificationForm.title.trim() || !notificationForm.message.trim()) {
      setMessage("Vui lòng nhập đầy đủ tiêu đề và nội dung thông báo");
      return;
    }

    if (sendType === 'class' && !selectedClassId) {
      setMessage("Vui lòng chọn lớp học");
      return;
    }

    if (sendType === 'individual' && selectedStudentIds.length === 0) {
      setMessage("Vui lòng chọn ít nhất một học viên");
      return;
    }

    setSaving(true);
    setMessage("Đang gửi thông báo...");

    try {
      if (sendType === 'all') {
        let allStudentIds: string[] = [];
        for (const cls of teacherClasses) {
          const result = await getLopHocStudents(cls.id);
          if (result.success && result.data?.danhSachHocVien) {
            const ids = result.data.danhSachHocVien
              .map((s: any) => s.taiKhoanId)
              .filter(Boolean);
            allStudentIds = [...allStudentIds, ...ids];
          }
        }

        allStudentIds = [...new Set(allStudentIds)];

        if (allStudentIds.length === 0) {
          setMessage("Không có học viên nào trong các lớp của bạn");
          setSaving(false);
          return;
        }

        let successCount = 0;
        let failCount = 0;

        for (const studentId of allStudentIds) {
          const result = await createThongBao({
            nguoiNhanId: studentId,
            tieuDe: notificationForm.title,
            noiDung: notificationForm.message,
          });
          if (result.success) {
            successCount++;
          } else {
            failCount++;
          }
        }

        if (successCount > 0) {
          setMessage(`Gửi thông báo thành công đến ${successCount} học viên${failCount > 0 ? ` (${failCount} thất bại)` : ''}`);
          setTimeout(() => {
            setShowNotificationModal(false);
            resetNotificationForm();
          }, 2000);
        } else {
          setMessage("Gửi thông báo thất bại");
        }

      } else {
        let recipientIds: string[] = [];

        if (sendType === 'class') {
          recipientIds = classStudents.map(s => s.taiKhoanId).filter(Boolean);
        } else if (sendType === 'individual') {
          recipientIds = selectedStudentIds;
        }

        if (recipientIds.length === 0) {
          setMessage("Không có người nhận nào được chọn");
          setSaving(false);
          return;
        }

        let successCount = 0;
        let failCount = 0;

        for (const recipientId of recipientIds) {
          const result = await createThongBao({
            nguoiNhanId: recipientId,
            tieuDe: notificationForm.title,
            noiDung: notificationForm.message,
          });
          if (result.success) {
            successCount++;
          } else {
            failCount++;
          }
        }

        if (successCount > 0) {
          setMessage(`Gửi thông báo thành công đến ${successCount} học viên${failCount > 0 ? ` (${failCount} thất bại)` : ''}`);
          setTimeout(() => {
            setShowNotificationModal(false);
            resetNotificationForm();
          }, 2000);
        } else {
          setMessage("Gửi thông báo thất bại");
        }
      }
    } catch (error) {
      console.error("Error sending notification:", error);
      setMessage("Có lỗi xảy ra khi gửi thông báo");
    } finally {
      setSaving(false);
    }
  };

  const resetNotificationForm = () => {
    setNotificationForm({ title: "", message: "" });
    setSendType('all');
    setSelectedClassId("");
    setSelectedStudentIds([]);
    setClassStudents([]);
    setMessage("");
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
          <p className="text-gray-600">Đang tải dữ liệu...</p>
        </div>
      </div>
    );
  }

  if (!profile) {
    return null;
  }

  const upcomingSchedules = lichHocs.flatMap(lich => {
    if (!lich.thoiGianBieu || lich.thoiGianBieu.length === 0) {
      return [{
        id: lich.id || "",
        thu: "—",
        tuNgay: lich.tuNgay,
        denNgay: lich.denNgay,
        gioBatDau: "—" as string | undefined,
        gioKetThuc: "—" as string | undefined,
        lopHoc: lich.lopHoc,
        phongHoc: lich.phongHoc
      }];
    }

    return lich.thoiGianBieu.map(tgb => ({
      id: `${lich.id || ""}-${tgb.id || ""}`,
      thu: getDayOfWeekText(tgb.thu),
      tuNgay: lich.tuNgay,
      denNgay: lich.denNgay,
      gioBatDau: tgb.gioBatDau,
      gioKetThuc: tgb.gioKetThuc,
      lopHoc: lich.lopHoc,
      phongHoc: lich.phongHoc
    }));
  });

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-blue-900 to-blue-700 rounded-xl p-8 text-white">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <div className="w-20 h-20 bg-white rounded-full flex items-center justify-center overflow-hidden">
              {profile.avatar ? (
                <img src={profile.avatar} alt="Avatar" className="w-full h-full object-cover" />
              ) : (
                <User className="w-10 h-10 text-gray-900" />
              )}
            </div>
            <div>
              <h1 className="text-3xl font-bold">{profile.email?.split('@')[0] || "Giáo viên"}</h1>
              <p className="text-blue-100 mt-1">Giáo viên</p>
            </div>
          </div>
          <button
            onClick={handleEditProfile}
            className="flex items-center space-x-2 bg-white text-blue-900 px-4 py-2 rounded-lg hover:bg-blue-50 transition-colors"
          >
            <Edit className="w-4 h-4" />
            <span>Chỉnh sửa</span>
          </button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Thông tin cá nhân */}
        <div className="lg:col-span-1">
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
              <User className="w-5 h-5 mr-2" />
              Thông tin cá nhân
            </h2>
            <div className="space-y-4">
              <div>
                <p className="text-sm text-gray-500 mb-1">Email</p>
                <div className="flex items-center text-gray-900">
                  <Mail className="w-4 h-4 mr-2 text-gray-400" />
                  {profile.email || "—"}
                </div>
              </div>
              <div>
                <p className="text-sm text-gray-500 mb-1">Số điện thoại</p>
                <div className="flex items-center text-gray-900">
                  <Phone className="w-4 h-4 mr-2 text-gray-400" />
                  {profile.sdt || "—"}
                </div>
              </div>
            </div>
          </div>

          {/* Thống kê giảng dạy */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mt-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
              <GraduationCap className="w-5 h-5 mr-2" />
              Thống kê giảng dạy
            </h2>
            <div className="space-y-4">
              <div className="bg-blue-50 rounded-lg p-4">
                <div className="flex items-center justify-between mb-2">
                  <span className="text-sm text-blue-700">Số lớp đang dạy</span>
                  <GraduationCap className="w-5 h-5 text-blue-600" />
                </div>
                <p className="text-2xl font-bold text-blue-900">{teacherClasses.length}</p>
              </div>
              <div className="bg-green-50 rounded-lg p-4">
                <div className="flex items-center justify-between mb-2">
                  <span className="text-sm text-green-700">Buổi dạy sắp tới</span>
                  <Clock className="w-5 h-5 text-green-600" />
                </div>
                <p className="text-2xl font-bold text-green-900">{upcomingSchedules.length}</p>
              </div>
            </div>
          </div>

          {/* Gửi thông báo */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mt-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
              <Bell className="w-5 h-5 mr-2" />
              Thông báo
            </h2>
            <p className="text-sm text-gray-600 mb-4">
              Gửi thông báo đến học viên trong lớp của bạn
            </p>
            <button
              onClick={() => setShowNotificationModal(true)}
              className="w-full flex items-center justify-center px-4 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors duration-200"
            >
              <Send className="w-5 h-5 mr-2" />
              Gửi thông báo mới
            </button>
          </div>
          </div>

          {/* Lịch dạy */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                <Calendar className="w-5 h-5 mr-2" />
                Lịch dạy sắp tới
              </h2>

              {upcomingSchedules.length === 0 ? (
                <div className="text-center py-8">
                  <Calendar className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                  <p className="text-gray-600">Không có lịch dạy nào trong thời gian tới</p>
                </div>
              ) : (
                <div className="space-y-3">
                  {upcomingSchedules.map((lh, index) => (
                    <div key={lh.id || index} className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow">
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <h3 className="font-semibold text-gray-900 mb-2">
                            {lh.lopHoc?.tenLop || "—"}
                          </h3>
                          <p className="text-xs text-gray-500 mb-3">
                            {lh.lopHoc?.khoaHoc?.tenKhoaHoc || "—"}
                          </p>
                          <div className="space-y-1 text-sm text-gray-600">
                            <div className="flex items-center">
                              <Calendar className="w-4 h-4 mr-2" />
                              {lh.thu} - {formatDate(lh.tuNgay)} đến {formatDate(lh.denNgay)}
                            </div>
                            <div className="flex items-center">
                              <Clock className="w-4 h-4 mr-2" />
                              {formatTime(lh.gioBatDau)} - {formatTime(lh.gioKetThuc)}
                            </div>
                            {lh.phongHoc?.tenPhong && (
                              <div className="flex items-center">
                                <MapPin className="w-4 h-4 mr-2" />
                                {lh.phongHoc.tenPhong}
                              </div>
                            )}
                          </div>
                        </div>
                        <div className="ml-4">
                          <span className="bg-green-100 text-green-800 text-xs font-semibold px-3 py-1 rounded-full">
                            Sắp diễn ra
                          </span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Edit Profile Modal */}
        {showEditModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-xl shadow-2xl max-w-md w-full">
              <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
                <h2 className="text-xl font-bold text-gray-900">Chỉnh sửa thông tin</h2>
                <button
                  onClick={() => setShowEditModal(false)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>

              <div className="p-6 space-y-6">
                {/* Avatar Upload */}
                <div className="flex flex-col items-center space-y-4">
                  <div className="relative">
                    <div className="w-32 h-32 bg-gray-100 rounded-full flex items-center justify-center overflow-hidden border-4 border-gray-200">
                      {editForm.avatar ? (
                        <img src={editForm.avatar} alt="Avatar" className="w-full h-full object-cover" />
                      ) : (
                        <User className="w-16 h-16 text-gray-400" />
                      )}
                    </div>
                    <label className="absolute bottom-0 right-0 bg-blue-600 text-white p-2 rounded-full cursor-pointer hover:bg-blue-700 transition-colors">
                      <Camera className="w-5 h-5" />
                      <input
                        type="file"
                        accept="image/*"
                        onChange={handleAvatarUpload}
                        className="hidden"
                        disabled={uploading}
                      />
                    </label>
                  </div>
                  {uploading && <p className="text-sm text-blue-600">Đang upload...</p>}
                </div>

                {/* Form Fields */}
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Email <span className="text-red-500">*</span>
                    </label>
                    <input
                      type="email"
                      value={editForm.email}
                      onChange={(e) => setEditForm(prev => ({ ...prev, email: e.target.value }))}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="Nhập email"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Số điện thoại
                    </label>
                    <input
                      type="tel"
                      value={editForm.sdt}
                      onChange={(e) => setEditForm(prev => ({ ...prev, sdt: e.target.value }))}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="Nhập số điện thoại"
                    />
                  </div>
                </div>

                {/* Message */}
                {message && (
                  <div className={`p-4 rounded-lg ${message.includes("thành công")
                    ? "bg-green-50 text-green-800 border border-green-200"
                    : "bg-red-50 text-red-800 border border-red-200"
                    }`}>
                    {message}
                  </div>
                )}

                {/* Action Buttons */}
                <div className="flex justify-end space-x-3 pt-4 border-t border-gray-200">
                  <button
                    onClick={() => setShowEditModal(false)}
                    className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                    disabled={saving}
                  >
                    Hủy
                  </button>
                  <button
                    onClick={handleSaveProfile}
                    className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                    disabled={saving || uploading}
                  >
                    {saving ? "Đang lưu..." : "Lưu thay đổi"}
                  </button>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Notification Modal */}
        {showNotificationModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
              <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
                <h2 className="text-2xl font-bold text-gray-900 flex items-center">
                  <Bell className="w-6 h-6 mr-2 text-blue-600" />
                  Gửi thông báo
                </h2>
                <button
                  onClick={() => {
                    setShowNotificationModal(false);
                    resetNotificationForm();
                  }}
                  className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>

              <div className="p-6 space-y-6">
                {/* Send Type Selection */}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-3">
                    Gửi đến <span className="text-red-500">*</span>
                  </label>
                  <div className="grid grid-cols-3 gap-3">
                    <button
                      type="button"
                      onClick={() => {
                        setSendType('all');
                        setSelectedClassId("");
                        setSelectedStudentIds([]);
                      }}
                      className={`p-4 rounded-lg border-2 transition-all ${sendType === 'all'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-300 hover:border-gray-400'
                        }`}
                    >
                      <Users className={`w-6 h-6 mx-auto mb-2 ${sendType === 'all' ? 'text-blue-600' : 'text-gray-600'}`} />
                      <p className={`text-sm font-medium ${sendType === 'all' ? 'text-blue-900' : 'text-gray-700'}`}>
                        Tất cả lớp
                      </p>
                    </button>

                    <button
                      type="button"
                      onClick={() => {
                        setSendType('class');
                        setSelectedStudentIds([]);
                      }}
                      className={`p-4 rounded-lg border-2 transition-all ${sendType === 'class'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-300 hover:border-gray-400'
                        }`}
                    >
                      <Users className={`w-6 h-6 mx-auto mb-2 ${sendType === 'class' ? 'text-blue-600' : 'text-gray-600'}`} />
                      <p className={`text-sm font-medium ${sendType === 'class' ? 'text-blue-900' : 'text-gray-700'}`}>
                        Theo lớp
                      </p>
                    </button>

                    <button
                      type="button"
                      onClick={() => setSendType('individual')}
                      className={`p-4 rounded-lg border-2 transition-all ${sendType === 'individual'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-300 hover:border-gray-400'
                        }`}
                    >
                      <User className={`w-6 h-6 mx-auto mb-2 ${sendType === 'individual' ? 'text-blue-600' : 'text-gray-600'}`} />
                      <p className={`text-sm font-medium ${sendType === 'individual' ? 'text-blue-900' : 'text-gray-700'}`}>
                        Chọn học viên
                      </p>
                    </button>
                  </div>
                </div>

                {/* Class Selection */}
                {(sendType === 'class' || sendType === 'individual') && (
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Chọn lớp học <span className="text-red-500">*</span>
                    </label>
                    <select
                      value={selectedClassId}
                      onChange={(e) => setSelectedClassId(e.target.value)}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    >
                      <option value="">-- Chọn lớp học --</option>
                      {teacherClasses.map((cls) => (
                        <option key={cls.id} value={cls.id}>
                          {cls.tenLop}
                        </option>
                      ))}
                    </select>
                  </div>
                )}

                {/* Student Selection */}
                {sendType === 'individual' && selectedClassId && (
                  <div>
                    <div className="flex items-center justify-between mb-2">
                      <label className="block text-sm font-semibold text-gray-700">
                        Chọn học viên <span className="text-red-500">*</span>
                      </label>
                      {classStudents.length > 0 && (
                        <button
                          type="button"
                          onClick={toggleAllStudents}
                          className="text-sm text-blue-600 hover:text-blue-800 font-medium"
                        >
                          {selectedStudentIds.length === classStudents.length ? 'Bỏ chọn tất cả' : 'Chọn tất cả'}
                        </button>
                      )}
                    </div>

                    <div className="border border-gray-300 rounded-lg max-h-64 overflow-y-auto">
                      {loadingClassStudents ? (
                        <div className="p-8 text-center">
                          <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
                          <p className="text-sm text-gray-600 mt-2">Đang tải danh sách học viên...</p>
                        </div>
                      ) : classStudents.length === 0 ? (
                        <div className="p-8 text-center text-gray-500">
                          Không có học viên nào trong lớp
                        </div>
                      ) : (
                        <div className="divide-y divide-gray-200">
                          {classStudents.map((student) => (
                            <label
                              key={student.id}
                              className="flex items-center p-3 hover:bg-gray-50 cursor-pointer"
                            >
                              <input
                                type="checkbox"
                                checked={selectedStudentIds.includes(student.taiKhoanId)}
                                onChange={() => toggleStudentSelection(student.taiKhoanId)}
                                className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                              />
                              <div className="ml-3 flex-1">
                                <p className="text-sm font-medium text-gray-900">{student.hoTen}</p>
                                <p className="text-xs text-gray-500">{student.email}</p>
                              </div>
                            </label>
                          ))}
                        </div>
                      )}
                    </div>
                    {selectedStudentIds.length > 0 && (
                      <p className="text-xs text-gray-600 mt-2">
                        Đã chọn {selectedStudentIds.length} / {classStudents.length} học viên
                      </p>
                    )}
                  </div>
                )}

                {/* Title */}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Tiêu đề thông báo <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text"
                    value={notificationForm.title}
                    onChange={(e) => setNotificationForm(prev => ({ ...prev, title: e.target.value }))}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Nhập tiêu đề thông báo"
                  />
                </div>

                {/* Message */}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Nội dung <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    value={notificationForm.message}
                    onChange={(e) => setNotificationForm(prev => ({ ...prev, message: e.target.value }))}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none"
                    rows={6}
                    placeholder="Nhập nội dung thông báo..."
                  />
                  <p className="text-xs text-gray-500 mt-1">
                    {notificationForm.message.length} ký tự
                  </p>
                </div>

                {/* Info box */}
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                  <p className="text-sm text-blue-800">
                    <strong>Lưu ý:</strong> {
                      sendType === 'all'
                        ? 'Thông báo sẽ được gửi đến tất cả học viên trong các lớp mà bạn đang giảng dạy.'
                        : sendType === 'class'
                          ? 'Thông báo sẽ được gửi đến tất cả học viên trong lớp đã chọn.'
                          : `Thông báo sẽ được gửi đến ${selectedStudentIds.length} học viên đã chọn.`
                    }
                  </p>
                </div>

                {/* Message */}
                {message && (
                  <div className={`p-4 rounded-lg ${message.includes("thành công")
                    ? "bg-green-50 text-green-800 border border-green-200"
                    : "bg-red-50 text-red-800 border border-red-200"
                    }`}>
                    {message}
                  </div>
                )}

                {/* Action Buttons */}
                <div className="flex justify-end space-x-3 pt-4 border-t border-gray-200">
                  <button
                    onClick={() => {
                      setShowNotificationModal(false);
                      resetNotificationForm();
                    }}
                    className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                    disabled={saving}
                  >
                    Hủy
                  </button>
                  <button
                    onClick={handleSendNotification}
                    className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                    disabled={saving || !notificationForm.title.trim() || !notificationForm.message.trim()}
                  >
                    <Send className="w-4 h-4 mr-2" />
                    {saving ? "Đang gửi..." : "Gửi thông báo"}
                  </button>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
      );
}
