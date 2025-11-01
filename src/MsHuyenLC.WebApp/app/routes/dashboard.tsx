import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { getProfile, updateProfile } from "~/apis/Profile";
import { getByTaiKhoanId, updateHocVien } from "~/apis/HocVien";
import { GioiTinh, TrangThaiTaiKhoan, VaiTro, type DeThi, type HocVien, type TaiKhoan, type KyThi } from "~/types/index";
import { getLichHocByStudent, getLichHocByTeacher } from "~/apis/LichHoc";
import { getPhanCongByLopHoc } from "~/apis/PhanCong";
import { getPhienLamBaiByHocVien } from "~/apis/PhienLamBai";
import { uploadAvatar } from "~/apis/Upload";
import { createThongBao } from "~/apis/ThongBao";
import { getHocViens } from "~/apis/HocVien";
import { getKyThis } from "~/apis/KyThi";
import {
  User, Calendar, BookOpen, Trophy, Clock,
  Mail, Phone, MapPin, CheckCircle,
  TrendingUp, Book, GraduationCap, Edit, X, Camera,
  TextCursor, Bell, Send, FileText
} from "lucide-react";

interface Profile {
  id?: string;
  tenDangNhap?: string;
  matkhau?: string;
  email?: string;
  sdt?: string;
  vaiTro?: VaiTro;
  trangThai?: TrangThaiTaiKhoan;
  avatar?: string;
  ngaytao?: string;
}

interface Student {
  id?: string;
  hoTen?: string;
  ngaySinh?: string;
  gioiTinh?: GioiTinh;
  diaChi?: string;
  ngayDangKy?: string;
  taiKhoanId?: string;
  taiKhoan?: TaiKhoan;
  trangThai?: TrangThaiTaiKhoan;
}

interface LichHoc {
  id?: string;
  ngayHoc?: string;
  thoiGianBatDau?: string;
  thoiGianKetThuc?: string;
  tenLopHoc?: string;
  tenPhongHoc?: string;
  tenGiaoVien?: string;
  thu?: number; 
  gioBatDau?: string;
  gioKetThuc?: string;
  tuNgay?: string;
  denNgay?: string;
  coHieuLuc?: boolean;
  lopHoc?: {
    id?: string;
    tenLop?: string;
    tenLopHoc?: string;
    khoaHoc?: {
      tenKhoaHoc?: string;
    };
  };
  phongHoc?: {
    id?: string;
    tenPhong?: string;
    tenPhongHoc?: string;
  };
}

interface PhienLamBai {
  id?: string;
  tongDiem?: number;
  ngayNop?: string;
  deThi?: DeThi;
}

export default function Dashboard() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [profile, setProfile] = useState<Profile>();
  const [student, setStudent] = useState<Student>();
  const [lichHocs, setLichHocs] = useState<LichHoc[]>([]);
  const [phienLamBais, setPhienLamBais] = useState<PhienLamBai[]>([]);
  const [kyThis, setKyThis] = useState<KyThi[]>([]);
  const [teachersByClass, setTeachersByClass] = useState<Record<string, string>>({});
  const [showEditModal, setShowEditModal] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState("");
  const [editForm, setEditForm] = useState({
    hoTen: "",
    email: "",
    sdt: "",
    diaChi: "",
    ngaySinh: "",
    gioiTinh: 0,
    avatar: ""
  });
  
  // Teacher notification states
  const [showNotificationModal, setShowNotificationModal] = useState(false);
  const [notificationForm, setNotificationForm] = useState({
    title: "",
    message: ""
  });

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    setLoading(true);

    const profileRes = await getProfile();
    if (!profileRes.success || !profileRes.data) {
      navigate("/dang-nhap");
      return;
    }

    setProfile(profileRes.data);

    if (profileRes.data.vaiTro === VaiTro.Admin || profileRes.data.vaiTro === VaiTro.GiaoVu) {
      navigate("/admin");
      return;
    }

    if (profileRes.data.vaiTro === VaiTro.HocVien) {
      const hocVienRes = await getByTaiKhoanId(profileRes.data.id!);
      if (hocVienRes.success && hocVienRes.data) {
        setStudent(hocVienRes.data);

        const lichHocRes = await getLichHocByStudent(hocVienRes.data.id!);
        if (lichHocRes.success && Array.isArray(lichHocRes.data)) {
          setLichHocs(lichHocRes.data);
          const teachersMap: Record<string, string> = {};
          const lopHocIds: string[] = [];
          
          for (const lich of lichHocRes.data) {
            if (lich.lopHoc?.id) {
              if (!teachersMap[lich.lopHoc.id]) {
                const phanCongRes = await getPhanCongByLopHoc(lich.lopHoc.id);
                if (phanCongRes.success && phanCongRes.data && phanCongRes.data.length > 0) {
                  teachersMap[lich.lopHoc.id] = phanCongRes.data[0].giaoVien?.hoTen || "";
                }
              }
              if (!lopHocIds.includes(lich.lopHoc.id)) {
                lopHocIds.push(lich.lopHoc.id);
              }
            }
          }
          setTeachersByClass(teachersMap);
          
          // Load exam periods for student's classes
          if (lopHocIds.length > 0) {
            const kyThiRes = await getKyThis({ pageNumber: 1, pageSize: 100 });
            if (kyThiRes.success && Array.isArray(kyThiRes.data)) {
              // Filter to get only exams from student's classes
              const studentKyThis = kyThiRes.data.filter((kt: KyThi) => 
                lopHocIds.includes(kt.lopHocId || '')
              );
              setKyThis(studentKyThis);
            }
          }
        }
      }

      const phienLamBai = await getPhienLamBaiByHocVien(hocVienRes.data.id!);
      if (phienLamBai.success && Array.isArray(phienLamBai.data)) {
        setPhienLamBais(phienLamBai.data);
      }

    } else if (profileRes.data.vaiTro === VaiTro.GiaoVien) {
      const lichDayRes = await getLichHocByTeacher(profileRes.data.id!);
      if (lichDayRes.success && Array.isArray(lichDayRes.data)) {
        setLichHocs(lichDayRes.data);
      }
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
    return timeString.substring(0, 5); // HH:mm
  };

  const isExamAvailable = (kyThi: KyThi): boolean => {
    if (!kyThi.ngayThi || !kyThi.gioBatDau || !kyThi.gioKetThuc) return false;
    
    const now = new Date();
    const currentDate = now.toISOString().split('T')[0]; // YYYY-MM-DD
    const currentTime = now.toTimeString().substring(0, 5); // HH:mm
    
    // Parse exam date
    const examDate = kyThi.ngayThi.split('T')[0];
    
    // Check if today is exam day
    if (currentDate !== examDate) return false;
    
    // Check if current time is within exam time range
    return currentTime >= kyThi.gioBatDau && currentTime <= kyThi.gioKetThuc;
  };

  const getExamStatus = (kyThi: KyThi): { text: string; color: string } => {
    if (!kyThi.ngayThi) return { text: "Chưa xác định", color: "bg-gray-100 text-gray-800" };
    
    const now = new Date();
    const currentDate = now.toISOString().split('T')[0];
    const examDate = kyThi.ngayThi.split('T')[0];
    
    if (currentDate < examDate) {
      return { text: "Sắp diễn ra", color: "bg-blue-100 text-blue-800" };
    } else if (currentDate === examDate) {
      if (isExamAvailable(kyThi)) {
        return { text: "Đang diễn ra", color: "bg-green-100 text-green-800" };
      }
      return { text: "Hôm nay", color: "bg-yellow-100 text-yellow-800" };
    } else {
      return { text: "Đã kết thúc", color: "bg-red-100 text-red-800" };
    }
  };

  const getGioiTinhText = (gioiTinh?: number) => {
    return gioiTinh === 0 ? "Nam" : "Nữ";
  };

  const getLoaiBaiThiText = (loaiBaiThi?: number) => {
    switch (loaiBaiThi) {
      case 0: return "Luyện tập";
      case 1: return "Thi chính thức";
      default: return "—";
    }
  };

  const getVaiTroText = (vaiTro?: VaiTro) => {
    switch (vaiTro) {
      case VaiTro.Admin: return "Quản trị viên";
      case VaiTro.GiaoVu: return "Giáo vụ";
      case VaiTro.GiaoVien: return "Giáo viên";
      case VaiTro.HocVien: return "Học viên";
      default: return "—";
    }
  };

  const handleEditProfile = () => {
    setEditForm({
      hoTen: student?.hoTen || "",
      email: profile?.email || "",
      sdt: profile?.sdt || "",
      diaChi: student?.diaChi || "",
      ngaySinh: student?.ngaySinh ? student.ngaySinh.split('T')[0] : "",
      gioiTinh: student?.gioiTinh || 0,
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

      if (profile?.vaiTro === VaiTro.HocVien && student?.id) {
        const studentUpdate = await updateHocVien(student.id, {
          hoTen: editForm.hoTen,
          diaChi: editForm.diaChi,
          ngaySinh: editForm.ngaySinh,
          gioiTinh: editForm.gioiTinh as GioiTinh
        });

        if (!studentUpdate.success) {
          setMessage(studentUpdate.message || "Cập nhật thông tin học viên thất bại");
          setSaving(false);
          return;
        }
      }

      setMessage("Cập nhật thông tin thành công");
      setTimeout(() => {
        setShowEditModal(false);
        loadDashboardData(); // Reload data
      }, 1500);
    } catch (error) {
      setMessage("Có lỗi xảy ra khi cập nhật thông tin");
    } finally {
      setSaving(false);
    }
  };

  const handleSendNotification = async () => {
    if (!notificationForm.title.trim() || !notificationForm.message.trim()) {
      setMessage("Vui lòng nhập đầy đủ tiêu đề và nội dung thông báo");
      return;
    }

    setSaving(true);
    setMessage("");

    try {
      // Get all students to send notification
      const studentsRes = await getHocViens({ pageNumber: 1, pageSize: 1000 });
      if (!studentsRes.success || !studentsRes.data || studentsRes.data.length === 0) {
        setMessage("Không tìm thấy học viên nào để gửi thông báo");
        setSaving(false);
        return;
      }

      // Get student IDs (TaiKhoanId)
      const studentIds = studentsRes.data
        .filter((s: any) => s.taiKhoanId)
        .map((s: any) => s.taiKhoanId!);

      if (studentIds.length === 0) {
        setMessage("Không có học viên nào để gửi thông báo");
        setSaving(false);
        return;
      }

      // Send notification via API
      const result = await createThongBao({
        tieuDe: notificationForm.title,
        noiDung: notificationForm.message,
      });

      if (result.success) {
        setMessage("Gửi thông báo thành công!");
        setTimeout(() => {
          setShowNotificationModal(false);
          setNotificationForm({ title: "", message: "" });
          setMessage("");
        }, 1500);
      } else {
        setMessage(result.message || "Gửi thông báo thất bại");
      }
    } catch (error) {
      setMessage("Có lỗi xảy ra khi gửi thông báo");
    } finally {
      setSaving(false);
    }
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

  var upcomingSchedules = lichHocs.map(lich => {
    return {
      id: lich.id,
      thu: getDayOfWeekText(lich.thu),
      tuNgay: lich.tuNgay,
      denNgay: lich.denNgay,
      gioBatDau: lich.gioBatDau,
      gioKetThuc: lich.gioKetThuc,
      lopHoc: lich.lopHoc,
      phongHoc: lich.phongHoc,
      giaoVien: lich.lopHoc?.id ? teachersByClass[lich.lopHoc.id] || "" : ""
    };
  });

  const isStudent = profile.vaiTro === VaiTro.HocVien;
  const isTeacher = profile.vaiTro === VaiTro.GiaoVien;


  // Thống kê bài thi (cho học sinh)
  const completedExams = phienLamBais.length;
  const averageScore = phienLamBais.length > 0
    ? phienLamBais.reduce((sum, bt) => sum + (bt.tongDiem || 0), 0) / phienLamBais.length
    : 0;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-gray-900 to-gray-700 rounded-xl p-8 text-white">
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
              <h1 className="text-3xl font-bold">{student?.hoTen || "Người dùng"}</h1>
              <p className="text-gray-200 mt-1">{getVaiTroText(profile.vaiTro)}</p>
            </div>
          </div>
          <button
            onClick={handleEditProfile}
            className="flex items-center space-x-2 bg-white text-gray-900 px-4 py-2 rounded-lg hover:bg-gray-100 transition-colors"
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
                <p className="text-sm text-gray-500 mb-1">Họ tên</p>
                <p className="text-gray-900 font-semibold">{student?.hoTen || "—"}</p>
              </div>
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
              <div>
                <p className="text-sm text-gray-500 mb-1">Địa chỉ</p>
                <div className="flex items-start text-gray-900">
                  <MapPin className="w-4 h-4 mr-2 mt-1 text-gray-400 flex-shrink-0" />
                  <span>{student?.diaChi || "—"}</span>
                </div>
              </div>
              <div>
                <p className="text-sm text-gray-500 mb-1">Ngày sinh</p>
                <p className="text-gray-900">{formatDate(student?.ngaySinh)}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500 mb-1">Giới tính</p>
                <p className="text-gray-900">{getGioiTinhText(student?.gioiTinh)}</p>
              </div>
            </div>
          </div>

          {/* Thống kê cho học sinh */}
          {isStudent && (
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mt-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                <Trophy className="w-5 h-5 mr-2" />
                Thành tích
              </h2>
              <div className="space-y-4">
                <div className="bg-blue-50 rounded-lg p-4">
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-sm text-blue-700">Bài thi đã làm</span>
                    <Book className="w-5 h-5 text-blue-600" />
                  </div>
                  <p className="text-2xl font-bold text-blue-900">{phienLamBais.length}</p>
                </div>
                <div className="bg-green-50 rounded-lg p-4">
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-sm text-green-700">Đã chấm điểm</span>
                    <CheckCircle className="w-5 h-5 text-green-600" />
                  </div>
                  <p className="text-2xl font-bold text-green-900">{completedExams}</p>
                </div>
                <div className="bg-purple-50 rounded-lg p-4">
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-sm text-purple-700">Điểm trung bình</span>
                    <TrendingUp className="w-5 h-5 text-purple-600" />
                  </div>
                  <p className="text-2xl font-bold text-purple-900">{averageScore.toFixed(1)}</p>
                </div>
              </div>
            </div>
          )}

          {/* Thống kê cho giáo viên */}
          {isTeacher && (
            <>
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mt-6">
                <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                  <GraduationCap className="w-5 h-5 mr-2" />
                  Thống kê giảng dạy
                </h2>
                <div className="space-y-4">
                  <div className="bg-blue-50 rounded-lg p-4">
                    <div className="flex items-center justify-between mb-2">
                      <span className="text-sm text-blue-700">Tổng buổi dạy</span>
                      <Calendar className="w-5 h-5 text-blue-600" />
                    </div>
                    <p className="text-2xl font-bold text-blue-900">{lichHocs.length}</p>
                  </div>
                  <div className="bg-green-50 rounded-lg p-4">
                    <div className="flex items-center justify-between mb-2">
                      <span className="text-sm text-green-700">Buổi sắp tới</span>
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
            </>
          )}
        </div>

        {/* Lịch học/dạy và bài thi */}
        <div className="lg:col-span-2 space-y-6">
          {/* Lịch học/dạy */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
              <Calendar className="w-5 h-5 mr-2" />
              {isStudent ? "Lịch học sắp tới" : "Lịch dạy sắp tới"}
            </h2>

            {upcomingSchedules.length === 0 ? (
              <div className="text-center py-8">
                <Calendar className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                <p className="text-gray-600">Không có lịch nào trong thời gian tới</p>
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
                          {(lh.phongHoc?.tenPhong) && (
                            <div className="flex items-center">
                              <MapPin className="w-4 h-4 mr-2" />
                              {lh.phongHoc?.tenPhong || "—"}
                            </div>
                          )}
                          {isStudent && (
                            <div className="flex items-center">
                              <User className="w-4 h-4 mr-2" />
                              GV: {lh.giaoVien || "—"}
                            </div>
                          )}
                        </div>
                      </div>
                      <div className="ml-4">
                        <span className="bg-blue-100 text-blue-800 text-xs font-semibold px-3 py-1 rounded-full">
                          Sắp diễn ra
                        </span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Lịch thi sắp tới (chỉ học sinh) */}
          {isStudent && kyThis.length > 0 && (
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                <FileText className="w-5 h-5 mr-2" />
                Lịch thi sắp tới
              </h2>

              <div className="space-y-3">
                {kyThis
                  .filter(kt => {
                    if (!kt.ngayThi) return false;
                    const examDate = new Date(kt.ngayThi.split('T')[0]);
                    const today = new Date();
                    today.setHours(0, 0, 0, 0);
                    return examDate >= today;
                  })
                  .sort((a, b) => {
                    const dateA = new Date(a.ngayThi || '');
                    const dateB = new Date(b.ngayThi || '');
                    return dateA.getTime() - dateB.getTime();
                  })
                  .slice(0, 5)
                  .map((kt, index) => {
                    const status = getExamStatus(kt);
                    const canTakeExam = isExamAvailable(kt);
                    
                    return (
                      <div key={kt.id || index} className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow">
                        <div className="flex items-start justify-between mb-3">
                          <div className="flex-1">
                            <h3 className="font-semibold text-gray-900 mb-1">
                              {kt.tenKyThi || "—"}
                            </h3>
                            <p className="text-xs text-gray-500">
                              {kt.lopHoc?.tenLop || "—"}
                            </p>
                          </div>
                          <span className={`text-xs font-semibold px-3 py-1 rounded-full ${status.color}`}>
                            {status.text}
                          </span>
                        </div>
                        
                        <div className="space-y-1 text-sm text-gray-600 mb-3">
                          <div className="flex items-center">
                            <Calendar className="w-4 h-4 mr-2" />
                            {formatDate(kt.ngayThi)}
                          </div>
                          {kt.gioBatDau && kt.gioKetThuc && (
                            <div className="flex items-center">
                              <Clock className="w-4 h-4 mr-2" />
                              {kt.gioBatDau} - {kt.gioKetThuc}
                            </div>
                          )}
                          {kt.thoiLuong && (
                            <div className="flex items-center">
                              <Clock className="w-4 h-4 mr-2" />
                              Thời lượng: {kt.thoiLuong} phút
                            </div>
                          )}
                        </div>

                        <button
                          onClick={() => {
                            if (canTakeExam) {
                              navigate(`/luyen-thi?kyThiId=${kt.id}`);
                            }
                          }}
                          disabled={!canTakeExam}
                          className={`w-full flex items-center justify-center px-4 py-2 rounded-lg font-semibold transition-colors ${
                            canTakeExam
                              ? 'bg-green-600 text-white hover:bg-green-700'
                              : 'bg-gray-200 text-gray-500 cursor-not-allowed'
                          }`}
                        >
                          <BookOpen className="w-4 h-4 mr-2" />
                          {canTakeExam ? 'Làm bài ngay' : 'Chưa đến giờ thi'}
                        </button>
                      </div>
                    );
                  })}
              </div>

              {kyThis.filter(kt => {
                if (!kt.ngayThi) return false;
                const examDate = new Date(kt.ngayThi.split('T')[0]);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                return examDate >= today;
              }).length === 0 && (
                <div className="text-center py-8">
                  <FileText className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                  <p className="text-gray-600">Không có kỳ thi nào sắp tới</p>
                </div>
              )}
            </div>
          )}

          {/* Bài thi đã làm (chỉ học sinh) */}
          {isStudent && (
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
                <BookOpen className="w-5 h-5 mr-2" />
                Bài thi gần đây
              </h2>

              {phienLamBais.length === 0 ? (
                <div className="text-center py-8">
                  <BookOpen className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                  <p className="text-gray-600">Bạn chưa làm bài thi nào</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-gray-50 border-b border-gray-200">
                      <tr>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Đề thi</th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Ngày nộp</th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Điểm</th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase">Loại bài thi</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-200">
                      {phienLamBais.slice(0, 10).map((plb, index) => (
                        <tr key={plb.id || index} className="hover:bg-gray-50">
                          <td className="px-4 py-3 text-sm text-gray-900">{plb.deThi?.tenDe || "—"}</td>
                          <td className="px-4 py-3 text-sm text-gray-600">{formatDate(plb.ngayNop)}</td>
                          <td className="px-4 py-3 text-sm">
                            {plb.tongDiem !== undefined && plb.tongDiem !== null ? (
                              <span className="font-semibold text-gray-900">{plb.tongDiem.toFixed(1)}</span>
                            ) : (
                              <span className="text-gray-400">—</span>
                            )}
                          </td>
                          <td className="px-4 py-3 text-sm">
                            <span className={`px-2 py-1 text-xs font-semibold rounded-full ${plb.deThi?.loaiDeThi === 1
                                ? "bg-green-100 text-green-800"
                                : "bg-yellow-100 text-yellow-800"
                              }`}>
                              {getLoaiBaiThiText(plb.deThi?.loaiDeThi)}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}
        </div>
      </div>

      {/* Edit Profile Modal */}
      {showEditModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
              <h2 className="text-xl font-bold text-gray-900">Chỉnh sửa thông tin cá nhân</h2>
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
                {uploading && (
                  <p className="text-sm text-blue-600">Đang upload...</p>
                )}
              </div>

              {/* Form Fields */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Họ tên <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text"
                    value={editForm.hoTen}
                    onChange={(e) => setEditForm(prev => ({ ...prev, hoTen: e.target.value }))}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Nhập họ tên"
                  />
                </div>

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

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Ngày sinh
                  </label>
                  <input
                    type="date"
                    value={editForm.ngaySinh}
                    onChange={(e) => setEditForm(prev => ({ ...prev, ngaySinh: e.target.value }))}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Giới tính
                  </label>
                  <select
                    value={editForm.gioiTinh}
                    onChange={(e) => setEditForm(prev => ({ ...prev, gioiTinh: Number(e.target.value) }))}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  >
                    <option value={0}>Nam</option>
                    <option value={1}>Nữ</option>
                  </select>
                </div>

                <div className="md:col-span-2">
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Địa chỉ
                  </label>
                  <input
                    type="text"
                    value={editForm.diaChi}
                    onChange={(e) => setEditForm(prev => ({ ...prev, diaChi: e.target.value }))}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Nhập địa chỉ"
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

      {/* Notification Modal for Teachers */}
      {showNotificationModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
              <h2 className="text-2xl font-bold text-gray-900 flex items-center">
                <Bell className="w-6 h-6 mr-2 text-blue-600" />
                Gửi thông báo
              </h2>
              <button
                onClick={() => setShowNotificationModal(false)}
                className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            <div className="p-6 space-y-6">
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
                  <strong>Lưu ý:</strong> Thông báo sẽ được gửi đến tất cả học viên trong các lớp mà bạn đang giảng dạy.
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
                  onClick={() => setShowNotificationModal(false)}
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
