import { Search, Plus, Edit, Trash2, Calendar, Clock, MapPin, X, AlertCircle } from "lucide-react";
import { useState, useEffect } from "react";
import { getLichHocs, createLichHoc, updateLichHoc, deleteLichHoc } from "~/apis/LichHoc";
import { getLopHocs } from "~/apis/LopHoc";
import { getPhongHocs } from "~/apis/PhongHoc";
import type { LichHoc, LichHocRequest, LichHocUpdateRequest, ThoiGianBieuRequest } from "~/types/schedule.types";
import type { LopHoc } from "~/types/course.types";
import type { PhongHoc } from "~/types/schedule.types";
import { DayOfWeek } from "~/types/enums";
import { setLightTheme } from "./_layout";
import Pagination from "~/components/Pagination";

export default function AdminSchedule() {
  const [searchTerm, setSearchTerm] = useState("");
  const [schedules, setSchedules] = useState<LichHoc[]>([]);
  const [classes, setClasses] = useState<LopHoc[]>([]);
  const [rooms, setRooms] = useState<PhongHoc[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingSchedule, setEditingSchedule] = useState<LichHoc | null>(null);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error">("success");
  const [conflictWarning, setConflictWarning] = useState("");
  
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  
  const [formData, setFormData] = useState<LichHocRequest>({
    lopHocId: "",
    phongHocId: "",
    thoiGianBieus: [
      {
        thu: DayOfWeek.Monday,
        gioBatDau: "08:00",
        gioKetThuc: "10:00",
      }
    ],
  });

  useEffect(() => {
    setLightTheme();
    loadSchedules();
  }, []);

  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm]);

  const loadSchedules = async () => {
    setLoading(true);
    try {
      // Load schedules, classes, and rooms in parallel
      const [scheduleResponse, classResponse, roomResponse] = await Promise.all([
        getLichHocs(),
        getLopHocs(),
        getPhongHocs()
      ]);
      
      if (scheduleResponse.success && scheduleResponse.data) {
        const dataArray = Array.isArray(scheduleResponse.data) ? scheduleResponse.data : [];
        
        // Create lookup maps for classes and rooms
        const classMap = new Map();
        const roomMap = new Map();
        
        if (classResponse.success && classResponse.data) {
          const classArray = Array.isArray(classResponse.data) ? classResponse.data : [];
          classArray.forEach((c: any) => classMap.set(c.id, c.tenLop));
          setClasses(classArray);
        }
        
        if (roomResponse.success && roomResponse.data) {
          const roomArray = Array.isArray(roomResponse.data) ? roomResponse.data : [];
          roomArray.forEach((r: any) => roomMap.set(r.id, r.tenPhong));
          setRooms(roomArray);
        }
        
        // Enrich schedule data with class and room names
        const enrichedSchedules = dataArray.map((schedule: any) => ({
          ...schedule,
          tenLop: schedule.tenLop || classMap.get(schedule.lopHocId) || "—",
          tenPhong: schedule.tenPhong || roomMap.get(schedule.phongHocId) || "—"
        }));
        
        console.log("Schedules loaded:", enrichedSchedules);
        setSchedules(enrichedSchedules);
      } else {
        console.error("Failed to load schedules:", scheduleResponse.message);
        setSchedules([]);
      }
    } catch (error) {
      console.error("Error loading schedules:", error);
      setSchedules([]);
    }
    setLoading(false);
  };



  const handleCreate = () => {
    setMessage("");
    setMessageType("success");
    setConflictWarning("");
    setEditingSchedule(null);
    setFormData({
      lopHocId: "",
      phongHocId: "",
      thoiGianBieus: [
        {
          thu: DayOfWeek.Monday,
          gioBatDau: "08:00",
          gioKetThuc: "10:00",
        }
      ],
    });
    setShowModal(true);
  };

  const handleEdit = (schedule: LichHoc) => {
    setMessage("");
    setMessageType("success");
    setConflictWarning("");
    setEditingSchedule(schedule);
    setFormData({
      phongHocId: schedule.phongHocId || "",
      thoiGianBieus: schedule.thoiGianBieu?.map(tgb => ({
        thu: tgb.thu || DayOfWeek.Monday,
        gioBatDau: tgb.gioBatDau?.substring(0, 5) || "08:00",
        gioKetThuc: tgb.gioKetThuc?.substring(0, 5) || "10:00",
      })) || [
        {
          thu: DayOfWeek.Monday,
          gioBatDau: "08:00",
          gioKetThuc: "10:00",
        }
      ],
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");

    if (editingSchedule) {
      const updateData: LichHocUpdateRequest = {
        phongHocId: formData.phongHocId,
        coHieuLuc: true,
        thoiGianBieus: formData.thoiGianBieus,
      };
      const response = await updateLichHoc(editingSchedule.id!, updateData);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadSchedules();
        setTimeout(() => setShowModal(false), 1500);
      }
    } else {
      const response = await createLichHoc(formData);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
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
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadSchedules();
      }
    }
  };

  const addThoiGianBieu = () => {
    setFormData({
      ...formData,
      thoiGianBieus: [
        ...(formData.thoiGianBieus || []),
        {
          thu: DayOfWeek.Monday,
          gioBatDau: "08:00",
          gioKetThuc: "10:00",
        }
      ]
    });
  };

  const removeThoiGianBieu = (index: number) => {
    setFormData({
      ...formData,
      thoiGianBieus: formData.thoiGianBieus?.filter((_, i) => i !== index)
    });
  };

  const updateThoiGianBieu = (index: number, field: keyof ThoiGianBieuRequest, value: any) => {
    const newThoiGianBieus = [...(formData.thoiGianBieus || [])];
    newThoiGianBieus[index] = {
      ...newThoiGianBieus[index],
      [field]: value
    };
    setFormData({
      ...formData,
      thoiGianBieus: newThoiGianBieus
    });
  };

  const checkScheduleConflict = () => {
    if (!formData.phongHocId || !formData.thoiGianBieus || formData.thoiGianBieus.length === 0) {
      setConflictWarning("");
      return;
    }

    const conflicts: string[] = [];

    formData.thoiGianBieus.forEach((newSlot) => {
      const conflictingSchedules = schedules.filter((schedule) => {
        if (editingSchedule && schedule.id === editingSchedule.id) {
          return false;
        }

        if (schedule.phongHocId !== formData.phongHocId) {
          return false;
        }

        return schedule.thoiGianBieu?.some((existingSlot) => {
          if (existingSlot.thu !== newSlot.thu) {
            return false;
          }

          const newStart = newSlot.gioBatDau || "00:00";
          const newEnd = newSlot.gioKetThuc || "23:59";
          const existingStart = existingSlot.gioBatDau?.substring(0, 5) || "00:00";
          const existingEnd = existingSlot.gioKetThuc?.substring(0, 5) || "23:59";

          const toMinutes = (time: string) => {
            const [hours, minutes] = time.split(':').map(Number);
            return hours * 60 + minutes;
          };

          const newStartMin = toMinutes(newStart);
          const newEndMin = toMinutes(newEnd);
          const existingStartMin = toMinutes(existingStart);
          const existingEndMin = toMinutes(existingEnd);

          return (
            (newStartMin < existingEndMin && newEndMin > existingStartMin) ||
            (existingStartMin < newEndMin && existingEndMin > newStartMin)
          );
        });
      });

      if (conflictingSchedules.length > 0) {
        conflictingSchedules.forEach((schedule) => {
          const roomName = schedule.tenPhong || schedule.phongHoc?.tenPhong || "phòng này";
          const className = schedule.tenLop || schedule.lopHoc?.tenLop || "một lớp khác";
          const dayName = getDayName(newSlot.thu!);
          conflicts.push(
            `${dayName} (${newSlot.gioBatDau} - ${newSlot.gioKetThuc}): ${roomName} đã được xếp cho lớp "${className}"`
          );
        });
      }
    });

    if (conflicts.length > 0) {
      setConflictWarning(`⚠️ Phát hiện trùng lịch:\n${conflicts.join('\n')}`);
    } else {
      setConflictWarning("");
    }
  };

  // Check for conflicts when room or time changes
  useEffect(() => {
    if (showModal) {
      checkScheduleConflict();
    }
  }, [formData.phongHocId, formData.thoiGianBieus, showModal]);

  const getDayName = (day: DayOfWeek) => {
    const days = {
      [DayOfWeek.Monday]: "Thứ 2",
      [DayOfWeek.Tuesday]: "Thứ 3",
      [DayOfWeek.Wednesday]: "Thứ 4",
      [DayOfWeek.Thursday]: "Thứ 5",
      [DayOfWeek.Friday]: "Thứ 6",
      [DayOfWeek.Saturday]: "Thứ 7",
      [DayOfWeek.Sunday]: "Chủ nhật",
    };
    return days[day] || "";
  };

  // Filter and pagination
  const filteredSchedules = schedules.filter(schedule =>
    schedule.tenLop?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    schedule.tenPhong?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const paginatedSchedules = filteredSchedules.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize
  );

  return (
    <div className="p-6">
      <div className="mb-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Quản lý Lịch học</h1>
            <p className="text-gray-600 mt-1">Quản lý lịch học và thời gian biểu</p>
          </div>
          <button
            onClick={handleCreate}
            className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-700 flex items-center gap-2 transition-colors"
          >
            <Plus size={20} />
            Tạo lịch học
          </button>
        </div>
      </div>

      {message && !showModal && messageType === "success" && (
        <div className="bg-green-50 border border-green-200 text-green-800 px-4 py-3 rounded-lg flex items-center justify-between">
          <span>{message}</span>
          <button onClick={() => setMessage("")} className="text-green-600 hover:text-green-800">
            <X className="w-4 h-4" />
          </button>
        </div>
      )}
      {message && !showModal && messageType === "error" && (
        <div className="bg-red-50 border border-red-200 text-red-800 px-4 py-3 rounded-lg flex items-start gap-3">
          <AlertCircle className="w-5 h-5 flex-shrink-0 mt-0.5" />
          <div className="flex-1">{message}</div>
          <button onClick={() => setMessage("")} className="text-red-600 hover:text-red-800">
            <X className="w-4 h-4" />
          </button> 
        </div>
      )}

      {/* Search */}
      <div className="mb-6 relative">
        <Search className="absolute left-3 top-3 text-gray-400" size={20} />
        <input
          type="text"
          placeholder="Tìm kiếm theo tên lớp, phòng..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </div>

      {/* Table */}
      {loading ? (
        <div className="text-center py-8">
          <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p className="mt-2 text-gray-600">Đang tải dữ liệu...</p>
        </div>
      ) : schedules.length === 0 ? (
        <div className="bg-white rounded-lg shadow p-8 text-center">
          <p className="text-gray-500 text-lg">Chưa có lịch học nào</p>
          <button
            onClick={handleCreate}
            className="mt-4 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
          >
            Tạo lịch học đầu tiên
          </button>
        </div>
      ) : (
        <>
          <div className="bg-white rounded-lg shadow overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Lớp học
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Phòng học
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thời gian
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thời gian biểu
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {paginatedSchedules.length > 0 ? (
                  paginatedSchedules.map((schedule) => (
                    <tr key={schedule.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-gray-900">
                          {schedule.tenLop || schedule.lopHoc?.tenLop || "—"}
                        </div>
                      </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center text-sm text-gray-500">
                        <MapPin size={16} className="mr-1" />
                        {schedule.tenPhong || schedule.phongHoc?.tenPhong || "—"}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center text-sm text-gray-500">
                        <Calendar size={16} className="mr-1" />
                        {schedule.tuNgay} - {schedule.denNgay}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-gray-900">
                        {schedule.thoiGianBieu && schedule.thoiGianBieu.length > 0 ? (
                          schedule.thoiGianBieu.map((tgb, index) => (
                            <div key={index} className="flex items-center gap-2 mb-1">
                              <span className="font-medium">{getDayName(tgb.thu!)}</span>
                              <Clock size={14} />
                              <span>{tgb.gioBatDau?.substring(0, 5)} - {tgb.gioKetThuc?.substring(0, 5)}</span>
                            </div>
                          ))
                        ) : (
                          <span className="text-gray-400">—</span>
                        )}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                        schedule.coHieuLuc
                          ? 'bg-green-100 text-green-800'
                          : 'bg-red-100 text-red-800'
                      }`}>
                        {schedule.coHieuLuc ? 'Có hiệu lực' : 'Không hiệu lực'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => handleEdit(schedule)}
                        className="text-blue-600 hover:text-blue-900 mr-4 inline-flex items-center"
                        title="Chỉnh sửa"
                      >
                        <Edit size={18} />
                      </button>
                      <button
                        onClick={() => handleDelete(schedule.id!)}
                        className="text-red-600 hover:text-red-900 inline-flex items-center"
                        title="Xóa"
                      >
                        <Trash2 size={18} />
                      </button>
                    </td>
                  </tr>
                ))
                ) : (
                  <tr>
                    <td colSpan={6} className="px-6 py-8 text-center text-gray-500">
                      Không tìm thấy lịch học nào
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>

          {filteredSchedules.length > pageSize && (
            <Pagination
              currentPage={currentPage}
              totalCount={filteredSchedules.length}
              pageSize={pageSize}
              onPageChange={setCurrentPage}
            />
          )}
        </>
      )}

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-2xl font-bold">
                  {editingSchedule ? 'Cập nhật lịch học' : 'Tạo lịch học mới'}
                </h2>
                <button
                  onClick={() => setShowModal(false)}
                  className="text-gray-500 hover:text-gray-700"
                >
                  <X size={24} />
                </button>
              </div>

              {message && messageType === "error" && (
                <div className="bg-red-50 border border-red-200 text-red-800 px-4 py-3 rounded-lg flex items-start gap-3 mb-4">
                  <AlertCircle className="w-5 h-5 flex-shrink-0 mt-0.5" />
                  <div className="flex-1">{message}</div>
                  <button onClick={() => setMessage("")} className="text-red-600 hover:text-red-800">
                    <X className="w-4 h-4" />
                  </button>
                </div>
              )}

              {conflictWarning && (
                <div className="bg-yellow-50 border border-yellow-200 text-yellow-800 px-4 py-3 rounded-lg flex items-start gap-3 mb-4">
                  <AlertCircle className="w-5 h-5 flex-shrink-0 mt-0.5" />
                  <div className="flex-1 whitespace-pre-line text-sm">{conflictWarning}</div>
                  <button onClick={() => setConflictWarning("")} className="text-yellow-600 hover:text-yellow-800">
                    <X className="w-4 h-4" />
                  </button>
                </div>
              )}

              <form onSubmit={handleSubmit} className="space-y-4">
                {!editingSchedule && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Lớp học <span className="text-red-500">*</span>
                    </label>
                    <select
                      required
                      value={formData.lopHocId}
                      onChange={(e) => setFormData({ ...formData, lopHocId: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    >
                      <option value="">-- Chọn lớp học --</option>
                      {classes.map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.tenLop}
                        </option>
                      ))}
                    </select>
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Phòng học <span className="text-red-500">*</span>
                  </label>
                  <select
                    required
                    value={formData.phongHocId}
                    onChange={(e) => setFormData({ ...formData, phongHocId: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  >
                    <option value="">-- Chọn phòng học --</option>
                    {rooms.map((r) => (
                      <option key={r.id} value={r.id}>
                        {r.tenPhong}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <div className="flex justify-between items-center mb-2">
                    <label className="block text-sm font-medium text-gray-700">
                      Thời gian biểu <span className="text-red-500">*</span>
                    </label>
                    <button
                      type="button"
                      onClick={addThoiGianBieu}
                      className="text-blue-600 hover:text-blue-800 text-sm flex items-center gap-1 font-medium"
                    >
                      <Plus size={16} />
                      Thêm buổi học
                    </button>
                  </div>

                  {formData.thoiGianBieus?.map((tgb, index) => (
                    <div key={index} className="border border-gray-200 rounded-lg p-4 mb-3">
                      <div className="flex justify-between items-center mb-3">
                        <span className="font-medium text-gray-700">Buổi học {index + 1}</span>
                        {(formData.thoiGianBieus?.length || 0) > 1 && (
                          <button
                            type="button"
                            onClick={() => removeThoiGianBieu(index)}
                            className="text-red-600 hover:text-red-800"
                          >
                            <X size={18} />
                          </button>
                        )}
                      </div>

                      <div className="grid grid-cols-3 gap-3">
                        <div>
                          <label className="block text-sm text-gray-600 mb-1">Thứ</label>
                          <select
                            required
                            value={tgb.thu}
                            onChange={(e) => updateThoiGianBieu(index, 'thu', parseInt(e.target.value))}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                          >
                            <option value={DayOfWeek.Monday}>Thứ 2</option>
                            <option value={DayOfWeek.Tuesday}>Thứ 3</option>
                            <option value={DayOfWeek.Wednesday}>Thứ 4</option>
                            <option value={DayOfWeek.Thursday}>Thứ 5</option>
                            <option value={DayOfWeek.Friday}>Thứ 6</option>
                            <option value={DayOfWeek.Saturday}>Thứ 7</option>
                            <option value={DayOfWeek.Sunday}>Chủ nhật</option>
                          </select>
                        </div>

                        <div>
                          <label className="block text-sm text-gray-600 mb-1">Giờ bắt đầu</label>
                          <input
                            type="time"
                            required
                            value={tgb.gioBatDau}
                            onChange={(e) => updateThoiGianBieu(index, 'gioBatDau', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                          />
                        </div>

                        <div>
                          <label className="block text-sm text-gray-600 mb-1">Giờ kết thúc</label>
                          <input
                            type="time"
                            required
                            value={tgb.gioKetThuc}
                            onChange={(e) => updateThoiGianBieu(index, 'gioKetThuc', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                          />
                        </div>
                      </div>
                    </div>
                  ))}
                </div>

                <div className="flex gap-3 pt-4 border-t">
                  <button
                    type="submit"
                    className="flex-1 bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors font-medium"
                  >
                    {editingSchedule ? 'Cập nhật' : 'Tạo mới'}
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowModal(false)}
                    className="flex-1 bg-gray-200 text-gray-800 py-2 px-4 rounded-lg hover:bg-gray-300 transition-colors font-medium"
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
