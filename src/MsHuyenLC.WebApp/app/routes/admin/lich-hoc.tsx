import { Search, Plus, Edit, Trash2, Calendar, Clock, MapPin, Users, X } from "lucide-react";
import { useState, useEffect } from "react";
import { getLichHocs, createLichHoc, updateLichHoc, deleteLichHoc } from "~/apis/LichHoc";
import { getLopHocs } from "~/apis/LopHoc";
import { getPhongHocs } from "~/apis/PhongHoc";
import type { LichHoc, LichHocRequest, LichHocUpdateRequest } from "~/types/schedule.types";
import type { LopHoc } from "~/types/course.types";
import type { PhongHoc } from "~/types/schedule.types";
import { DayOfWeek } from "~/types/enums";
import { setLightTheme } from "./_layout";

export default function AdminSchedule() {
  const [searchTerm, setSearchTerm] = useState("");
  const [schedules, setSchedules] = useState<LichHoc[]>([]);
  const [classes, setClasses] = useState<LopHoc[]>([]);
  const [rooms, setRooms] = useState<PhongHoc[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingSchedule, setEditingSchedule] = useState<LichHoc | null>(null);
  const [message, setMessage] = useState("");
  
  const [formData, setFormData] = useState<LichHocRequest>({
    lopHocId: "",
    phongHocId: "",
    thu: DayOfWeek.Monday,
    gioBatDau: "08:00",
    gioKetThuc: "10:00",
    tuNgay: "",
    denNgay: "",
  });

  useEffect(() => {
    setLightTheme();
    loadSchedules();
    loadClasses();
    loadRooms();
  }, []);

  const loadSchedules = async () => {
    setLoading(true);
    const response = await getLichHocs({ pageNumber: 1, pageSize: 100 });
    if (response.success && response.data) {
      const dataArray = Array.isArray(response.data) ? response.data : (response.data.items || response.data.data || []);
      setSchedules(dataArray);
    }
    setLoading(false);
  };

  const loadClasses = async () => {
    const response = await getLopHocs({ pageNumber: 1, pageSize: 100 });
    if (response.success && response.data) {
      setClasses(response.data);
    }
  };

  const loadRooms = async () => {
    const response = await getPhongHocs({ pageNumber: 1, pageSize: 100 });
    if (response.success && response.data) {
      setRooms(response.data);
    }
  };

  const handleCreate = () => {
    setEditingSchedule(null);
    setFormData({
      lopHocId: "",
      phongHocId: "",
      thu: DayOfWeek.Monday,
      gioBatDau: "08:00",
      gioKetThuc: "10:00",
      tuNgay: "",
      denNgay: "",
    });
    setShowModal(true);
    setMessage("");
  };

  const handleEdit = (schedule: LichHoc) => {
    setEditingSchedule(schedule);
    setFormData({
      lopHocId: schedule.lopHocId || "",
      phongHocId: schedule.phongHocId || "",
      thu: schedule.thu || DayOfWeek.Monday,
      gioBatDau: schedule.gioBatDau || "08:00",
      gioKetThuc: schedule.gioKetThuc || "10:00",
      tuNgay: schedule.tuNgay ? schedule.tuNgay.split('T')[0] : "",
      denNgay: schedule.denNgay ? schedule.denNgay.split('T')[0] : "",
    });
    setShowModal(true);
    setMessage("");
  };

  const checkScheduleConflict = () => {
    const conflicts: string[] = [];
    
    // Parse time strings to compare
    const parseTime = (timeStr: string) => {
      const [hours, minutes] = timeStr.split(':').map(Number);
      return hours * 60 + minutes;
    };
    
    const newStart = parseTime(formData.gioBatDau || "00:00");
    const newEnd = parseTime(formData.gioKetThuc || "00:00");
    
    // Check time overlap function
    const isTimeOverlap = (start1: number, end1: number, start2: number, end2: number) => {
      return start1 < end2 && start2 < end1;
    };
    
    // Check date range overlap function
    const isDateRangeOverlap = (start1: string, end1: string, start2?: string, end2?: string) => {
      if (!start2 || !end2) return false;
      const date1Start = new Date(start1);
      const date1End = new Date(end1);
      const date2Start = new Date(start2);
      const date2End = new Date(end2);
      return date1Start <= date2End && date2Start <= date1End;
    };
    
    // Filter schedules for the same day and overlapping date range
    const relevantSchedules = schedules.filter(schedule => {
      // Skip the schedule being edited
      if (editingSchedule && schedule.id === editingSchedule.id) return false;
      
      // Must be same day of week
      if (schedule.thu !== formData.thu) return false;
      
      // Check if date ranges overlap
      if (formData.tuNgay && formData.denNgay) {
        return isDateRangeOverlap(formData.tuNgay, formData.denNgay, schedule.tuNgay, schedule.denNgay);
      }
      
      return true;
    });
    
    relevantSchedules.forEach(schedule => {
      const scheduleStart = parseTime(schedule.gioBatDau || "00:00");
      const scheduleEnd = parseTime(schedule.gioKetThuc || "00:00");
      
      if (isTimeOverlap(newStart, newEnd, scheduleStart, scheduleEnd)) {
        // Check room conflict
        if (schedule.phongHocId === formData.phongHocId) {
          const roomName = rooms.find(r => r.id === formData.phongHocId)?.tenPhong || "phòng này";
          const className = schedule.lopHoc?.tenLop || "một lớp khác";
          const dateRange = schedule.tuNgay && schedule.denNgay 
            ? ` (${formatDate(schedule.tuNgay)} - ${formatDate(schedule.denNgay)})`
            : "";
          conflicts.push(`Phòng ${roomName} đã có lịch học của ${className} vào ${getDayOfWeekText(schedule.thu)} ${schedule.gioBatDau}-${schedule.gioKetThuc}${dateRange}`);
        }
        
        // Check class conflict
        if (schedule.lopHocId === formData.lopHocId) {
          const className = classes.find(c => c.id === formData.lopHocId)?.tenLop || "lớp này";
          const roomName = schedule.phongHoc?.tenPhong || "một phòng khác";
          const dateRange = schedule.tuNgay && schedule.denNgay 
            ? ` (${formatDate(schedule.tuNgay)} - ${formatDate(schedule.denNgay)})`
            : "";
          conflicts.push(`Lớp ${className} đã có lịch học tại ${roomName} vào ${getDayOfWeekText(schedule.thu)} ${schedule.gioBatDau}-${schedule.gioKetThuc}${dateRange}`);
        }
      }
    });
    
    return conflicts;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate time
    if (formData.gioBatDau && formData.gioKetThuc && formData.gioBatDau >= formData.gioKetThuc) {
      setMessage("❌ Thời gian kết thúc phải sau thời gian bắt đầu");
      return;
    }
    
    // Validate date range
    if (formData.tuNgay && formData.denNgay && formData.tuNgay > formData.denNgay) {
      setMessage("❌ Ngày kết thúc phải sau ngày bắt đầu");
      return;
    }
    
    // Check for conflicts
    const conflicts = checkScheduleConflict();
    if (conflicts.length > 0) {
      setMessage("⚠️ Cảnh báo trùng lịch:\n" + conflicts.join("\n"));
      return;
    }
    
    if (editingSchedule) {
      const updateData: LichHocUpdateRequest = {
        phongHocId: formData.phongHocId,
        thu: formData.thu,
        gioBatDau: formData.gioBatDau,
        gioKetThuc: formData.gioKetThuc,
        tuNgay: formData.tuNgay,
        denNgay: formData.denNgay,
        coHieuLuc: true,
      };
      const response = await updateLichHoc(editingSchedule.id!, updateData);
      setMessage(response.message || "");
      if (response.success) {
        loadSchedules();
        setTimeout(() => setShowModal(false), 1500);
      }
    } else {
      const response = await createLichHoc(formData);
      setMessage(response.message || "");
      if (response.success) {
        loadSchedules();
        setTimeout(() => setShowModal(false), 1500);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa lịch học này?")) {
      const response = await deleteLichHoc(id);
      setMessage(response.message || "");
      if (response.success) {
        loadSchedules();
      }
    }
  };

  const getDayOfWeekText = (day?: DayOfWeek) => {
    const days = ["Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7"];
    return days[day || 0];
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return "—";
    const date = new Date(dateString);
    return date.toLocaleDateString("vi-VN");
  };

  const formatTime = (timeString?: string) => {
    if (!timeString) return "—";
    // Handle both HH:mm:ss and HH:mm formats
    return timeString.substring(0, 5);
  };

  const formatDateRange = (tuNgay?: string, denNgay?: string) => {
    if (!tuNgay || !denNgay) return "—";
    return `${formatDate(tuNgay)} - ${formatDate(denNgay)}`;
  };

  const filteredSchedules = schedules.filter(schedule => {
    if (!searchTerm) return true;
    const classNameMatch = schedule.lopHoc?.tenLop?.toLowerCase().includes(searchTerm.toLowerCase());
    const roomNameMatch = schedule.phongHoc?.tenPhong?.toLowerCase().includes(searchTerm.toLowerCase());
    return classNameMatch || roomNameMatch;
  });

  return (
    <div className="space-y-6">
      {message && (
        <div className={`px-4 py-3 rounded-lg ${
          message.includes("thành công") 
            ? "bg-green-100 border border-green-400 text-green-700"
            : message.includes("Cảnh báo")
            ? "bg-yellow-100 border border-yellow-400 text-yellow-800"
            : "bg-red-100 border border-red-400 text-red-700"
        }`}>
          <div className="whitespace-pre-line">{message}</div>
        </div>
      )}

      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý lịch học</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả lịch học</p>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm lịch học</span>
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm theo lớp học hoặc phòng học..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          />
        </div>
      </div>

      {loading && (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
        </div>
      )}

      {!loading && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Lớp học
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Phòng học
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thứ
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thời gian
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Khoảng thời gian
                  </th>
                  <th className="px-6 py-4 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredSchedules.map((schedule) => (
                  <tr key={schedule.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <Users className="w-5 h-5 text-gray-400 mr-2" />
                        <span className="font-medium text-gray-900">
                          {classes.find(l => l.id === schedule.lopHocId)?.tenLop || "—"}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <MapPin className="w-5 h-5 text-gray-400 mr-2" />
                        <span className="text-gray-700">
                          {rooms.find(r => r.id === schedule.phongHocId)?.tenPhong || "—"}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <Calendar className="w-5 h-5 text-gray-400 mr-2" />
                        <span className="text-gray-700">
                          {getDayOfWeekText(schedule.thu)}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <Clock className="w-5 h-5 text-gray-400 mr-2" />
                        <span className="text-gray-700">
                          {formatTime(schedule.gioBatDau)} - {formatTime(schedule.gioKetThuc)}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm text-gray-600">
                        {formatDateRange(schedule.tuNgay, schedule.denNgay)}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex justify-end space-x-2">
                        <button 
                          onClick={() => handleEdit(schedule)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Chỉnh sửa"
                        >
                          <Edit className="w-5 h-5" />
                        </button>
                        <button 
                          onClick={() => handleDelete(schedule.id!)}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                          title="Xóa"
                        >
                          <Trash2 className="w-5 h-5" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
                {filteredSchedules.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-gray-500">
                      <Calendar className="w-12 h-12 mx-auto mb-3 text-gray-400" />
                      <p>Không tìm thấy lịch học nào</p>
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingSchedule ? "Chỉnh sửa lịch học" : "Thêm lịch học mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Lớp học <span className="text-red-500">*</span>
                  </label>
                  <select
                    value={formData.lopHocId}
                    onChange={(e) => setFormData({ ...formData, lopHocId: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                    disabled={!!editingSchedule}
                  >
                    <option value="">Chọn lớp học</option>
                    {classes.map((cls) => (
                      <option key={cls.id} value={cls.id}>
                        {cls.tenLop}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Phòng học <span className="text-red-500">*</span>
                  </label>
                  <select
                    value={formData.phongHocId}
                    onChange={(e) => setFormData({ ...formData, phongHocId: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  >
                    <option value="">Chọn phòng học</option>
                    {rooms.map((room) => (
                      <option key={room.id} value={room.id}>
                        {room.tenPhong} (Sức chứa: {room.soGhe})
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Thứ <span className="text-red-500">*</span>
                  </label>
                  <select
                    value={formData.thu}
                    onChange={(e) => setFormData({ ...formData, thu: Number(e.target.value) as DayOfWeek })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  >
                    <option value={DayOfWeek.Sunday}>Chủ nhật</option>
                    <option value={DayOfWeek.Monday}>Thứ 2</option>
                    <option value={DayOfWeek.Tuesday}>Thứ 3</option>
                    <option value={DayOfWeek.Wednesday}>Thứ 4</option>
                    <option value={DayOfWeek.Thursday}>Thứ 5</option>
                    <option value={DayOfWeek.Friday}>Thứ 6</option>
                    <option value={DayOfWeek.Saturday}>Thứ 7</option>
                  </select>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Giờ bắt đầu <span className="text-red-500">*</span>
                    </label>
                    <input
                      type="time"
                      value={formData.gioBatDau}
                      onChange={(e) => setFormData({ ...formData, gioBatDau: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Giờ kết thúc <span className="text-red-500">*</span>
                    </label>
                    <input
                      type="time"
                      value={formData.gioKetThuc}
                      onChange={(e) => setFormData({ ...formData, gioKetThuc: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Từ ngày <span className="text-red-500">*</span>
                    </label>
                    <input
                      type="date"
                      value={formData.tuNgay}
                      onChange={(e) => setFormData({ ...formData, tuNgay: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Đến ngày <span className="text-red-500">*</span>
                    </label>
                    <input
                      type="date"
                      value={formData.denNgay}
                      onChange={(e) => setFormData({ ...formData, denNgay: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                </div>

                {/* Preview conflicts */}
                {formData.phongHocId && formData.lopHocId && formData.gioBatDau && formData.gioKetThuc && (() => {
                  const conflicts = checkScheduleConflict();
                  if (conflicts.length > 0) {
                    return (
                      <div className="bg-yellow-50 border border-yellow-300 rounded-lg p-4">
                        <div className="flex items-start">
                          <div className="flex-shrink-0">
                            <svg className="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                              <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                            </svg>
                          </div>
                          <div className="ml-3 flex-1">
                            <h3 className="text-sm font-medium text-yellow-800">Phát hiện trùng lịch</h3>
                            <div className="mt-2 text-sm text-yellow-700">
                              <ul className="list-disc list-inside space-y-1">
                                {conflicts.map((conflict, index) => (
                                  <li key={index}>{conflict}</li>
                                ))}
                              </ul>
                            </div>
                          </div>
                        </div>
                      </div>
                    );
                  }
                  return null;
                })()}

                {/* Error/Success message */}
                {message && (
                  <div className={`rounded-lg p-4 ${
                    message.includes("thành công") 
                      ? "bg-green-50 text-green-800 border border-green-200"
                      : "bg-red-50 text-red-800 border border-red-200"
                  }`}>
                    <div className="whitespace-pre-line text-sm">{message}</div>
                  </div>
                )}

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingSchedule ? "Cập nhật" : "Thêm mới"}
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowModal(false)}
                    className="flex-1 bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors"
                  >
                    Hủy
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
