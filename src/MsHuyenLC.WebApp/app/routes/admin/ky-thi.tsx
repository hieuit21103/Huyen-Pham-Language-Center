import { Search, Plus, Edit, Trash2, Calendar, X, Play, Pause, CheckCircle, PlusCircle, MinusCircle } from "lucide-react";
import { useState, useEffect } from "react";
import { getKyThis, createKyThi, updateKyThi, deleteKyThi, updateKyThiStatus } from "~/apis/KyThi";
import { getLopHocs } from "~/apis/LopHoc";
import type { KyThi, TrangThaiKyThi, CauHinhKyThiRequest, CapDo, DoKho, KyNang } from "~/types/index";
import { formatDateForInput, formatDateForDisplay } from "~/utils/date-utils";
import { setLightTheme } from "./_layout";

export default function AdminExamPeriods() {
  const [searchTerm, setSearchTerm] = useState("");
  const [examPeriods, setExamPeriods] = useState<KyThi[]>([]);
  const [classes, setClasses] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingPeriod, setEditingPeriod] = useState<KyThi | null>(null);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error">("success");
  
  const [formData, setFormData] = useState({
    tenKyThi: "",
    lopHocId: "",
    ngayThi: "",
    gioBatDau: "",
    gioKetThuc: "",
    thoiLuong: 60,
    cauHinhKyThis: [] as CauHinhKyThiRequest[],
  });

  useEffect(() => {
    setLightTheme();
    loadExamPeriods();
    loadClasses();
  }, []);

  const loadExamPeriods = async () => {
    setLoading(true);
    const response = await getKyThis();
    if (response.success && response.data) {
      setExamPeriods(response.data);
    }
    setLoading(false);
  };

  const loadClasses = async () => {
    const response = await getLopHocs();
    if (response.success && response.data) {
      setClasses(response.data);
    }
  };

  const handleCreate = () => {
    setEditingPeriod(null);
    setMessage("");
    setFormData({
      tenKyThi: "",
      lopHocId: "",
      ngayThi: "",
      gioBatDau: "",
      gioKetThuc: "",
      thoiLuong: 60,
      cauHinhKyThis: [],
    });
    setShowModal(true);
  };

  const handleEdit = (period: KyThi) => {
    setEditingPeriod(period);
    setMessage("");
    setFormData({
      tenKyThi: period.tenKyThi || "",
      lopHocId: period.lopHocId || "",
      ngayThi: formatDateForInput(period.ngayThi),
      gioBatDau: period.gioBatDau || "",
      gioKetThuc: period.gioKetThuc || "",
      thoiLuong: period.thoiLuong || 60,
      cauHinhKyThis: [],
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (editingPeriod) {
      const response = await updateKyThi(editingPeriod.id!, formData);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadExamPeriods();
        setShowModal(false);
      }
    } else {
      const response = await createKyThi(formData);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadExamPeriods();
        setShowModal(false);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa kỳ thi này?")) {
      const response = await deleteKyThi(id);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadExamPeriods();
      }
    }
  };

  const handleStatusChange = async (id: string, status: TrangThaiKyThi) => {
    const response = await updateKyThiStatus(id, status);
    setMessage(response.message || "");
    setMessageType(response.success ? "success" : "error");
    if (response.success) {
      loadExamPeriods();
    }
  };

  const getTrangThaiText = (trangThai?: TrangThaiKyThi) => {
    switch (trangThai) {
      case 0: return "Sắp diễn ra";
      case 1: return "Đang diễn ra";
      case 2: return "Kết thúc";
      case 3: return "Đã hủy";
      default: return "Không xác định";
    }
  };

  const getTrangThaiColor = (trangThai?: TrangThaiKyThi) => {
    switch (trangThai) {
      case 0: return "bg-blue-100 text-blue-800";
      case 1: return "bg-green-100 text-green-800";
      case 2: return "bg-gray-100 text-gray-800";
      case 3: return "bg-red-100 text-red-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const addCauHinh = () => {
    setFormData({
      ...formData,
      cauHinhKyThis: [
        ...formData.cauHinhKyThis,
        {
          capDo: 0,
          doKho: 0,
          kyNang: 0,
          cheDoCauHoi: 0,
          soCauHoi: 10,
        },
      ],
    });
  };

  const removeCauHinh = (index: number) => {
    setFormData({
      ...formData,
      cauHinhKyThis: formData.cauHinhKyThis.filter((_, i) => i !== index),
    });
  };

  const updateCauHinh = (index: number, field: keyof CauHinhKyThiRequest, value: any) => {
    const updated = [...formData.cauHinhKyThis];
    updated[index] = { ...updated[index], [field]: value };
    setFormData({ ...formData, cauHinhKyThis: updated });
  };

  const filteredPeriods = examPeriods.filter(period =>
    period.tenKyThi?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {message && messageType === "success" && (
        <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg">
          {message}
        </div>
      )}

      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý kỳ thi</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả kỳ thi</p>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm kỳ thi</span>
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm kỳ thi..."
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
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredPeriods.map((period) => (
            <div key={period.id} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              <div className="p-6">
                <div className="flex items-start justify-between mb-4">
                  <div className="w-12 h-12 bg-gray-900 rounded-lg flex items-center justify-center">
                    <Calendar className="w-6 h-6 text-white" />
                  </div>
                  <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${getTrangThaiColor(period.trangThai)}`}>
                    {getTrangThaiText(period.trangThai)}
                  </span>
                </div>
                
                <h3 className="text-xl font-bold text-gray-900 mb-2 line-clamp-2 min-h-[3.5rem]">{period.tenKyThi}</h3>
                <p className="text-sm text-gray-600 mb-4 line-clamp-1">{period.lopHoc?.tenLop}</p>
                
                <div className="space-y-2 mb-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Ngày thi:</span>
                    <span className="font-semibold text-gray-900">
                      {formatDateForDisplay(period.ngayThi)}
                    </span>
                  </div>
                  {period.gioBatDau && (
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-600">Giờ bắt đầu:</span>
                      <span className="font-semibold text-gray-900">{period.gioBatDau}</span>
                    </div>
                  )}
                  {period.gioKetThuc && (
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-600">Giờ kết thúc:</span>
                      <span className="font-semibold text-gray-900">{period.gioKetThuc}</span>
                    </div>
                  )}
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Thời lượng:</span>
                    <span className="font-semibold text-gray-900">{period.thoiLuong || 0} phút</span>
                  </div>
                </div>
                
                <div className="space-y-2 mb-4">
                  <div className="flex space-x-2">
                    {period.trangThai === 0 && (
                      <button 
                        onClick={() => handleStatusChange(period.id!, 1)}
                        className="flex-1 bg-green-600 text-white px-3 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center justify-center space-x-2 text-sm"
                      >
                        <Play className="w-4 h-4" />
                        <span>Bắt đầu thi</span>
                      </button>
                    )}
                    {period.trangThai === 1 && (
                      <button 
                        onClick={() => handleStatusChange(period.id!, 2)}
                        className="flex-1 bg-gray-600 text-white px-3 py-2 rounded-lg hover:bg-gray-700 transition-colors flex items-center justify-center space-x-2 text-sm"
                      >
                        <CheckCircle className="w-4 h-4" />
                        <span>Kết thúc</span>
                      </button>
                    )}
                    {(period.trangThai === 0 || period.trangThai === 1) && (
                      <button 
                        onClick={() => handleStatusChange(period.id!, 3)}
                        className="flex-1 bg-red-600 text-white px-3 py-2 rounded-lg hover:bg-red-700 transition-colors flex items-center justify-center space-x-2 text-sm"
                      >
                        <X className="w-4 h-4" />
                        <span>Hủy</span>
                      </button>
                    )}
                  </div>
                </div>
                
                <div className="flex space-x-2 pt-4 border-t border-gray-200">
                  <button 
                    onClick={() => handleEdit(period)}
                    className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center space-x-2"
                  >
                    <Edit className="w-4 h-4" />
                    <span>Sửa</span>
                  </button>
                  <button 
                    onClick={() => handleDelete(period.id!)}
                    className="flex-1 bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors flex items-center justify-center space-x-2"
                  >
                    <Trash2 className="w-4 h-4" />
                    <span>Xóa</span>
                  </button>
                </div>
              </div>
            </div>
          ))}
          </div>
        </>
      )}

      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingPeriod ? "Chỉnh sửa kỳ thi" : "Thêm kỳ thi mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tên kỳ thi</label>
                  <input
                    type="text"
                    value={formData.tenKyThi}
                    onChange={(e) => setFormData({ ...formData, tenKyThi: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Lớp học</label>
                  <select
                    value={formData.lopHocId}
                    onChange={(e) => setFormData({ ...formData, lopHocId: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  >
                    <option value="">-- Chọn lớp học --</option>
                    {classes.map((cls) => (
                      <option key={cls.id} value={cls.id}>
                        {cls.tenLop}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Ngày thi</label>
                  <input
                    type="date"
                    value={formData.ngayThi}
                    onChange={(e) => setFormData({ ...formData, ngayThi: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Giờ bắt đầu</label>
                    <input
                      type="time"
                      value={formData.gioBatDau}
                      onChange={(e) => setFormData({ ...formData, gioBatDau: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Giờ kết thúc</label>
                    <input
                      type="time"
                      value={formData.gioKetThuc}
                      onChange={(e) => setFormData({ ...formData, gioKetThuc: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Thời lượng (phút)</label>
                  <input
                    type="number"
                    value={formData.thoiLuong}
                    onChange={(e) => setFormData({ ...formData, thoiLuong: Number(e.target.value) })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                    min="1"
                  />
                </div>

                {!editingPeriod && (
                  <div>
                    <div className="flex justify-between items-center mb-3">
                      <label className="block text-sm font-medium text-gray-700">Cấu hình đề thi</label>
                      <button
                        type="button"
                        onClick={addCauHinh}
                        className="text-sm bg-blue-600 text-white px-3 py-1 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-1"
                      >
                        <PlusCircle className="w-4 h-4" />
                        <span>Thêm cấu hình</span>
                      </button>
                    </div>

                    {formData.cauHinhKyThis.length === 0 && (
                      <div className="text-sm text-gray-500 italic border border-gray-200 rounded-lg p-4 text-center">
                        Chưa có cấu hình. Nhấn "Thêm cấu hình" để thêm cấu hình đề thi.
                      </div>
                    )}

                    <div className="space-y-3">
                      {formData.cauHinhKyThis.map((cauHinh, index) => (
                        <div key={index} className="border border-gray-200 rounded-lg p-4 bg-gray-50">
                          <div className="flex justify-between items-center mb-3">
                            <span className="text-sm font-semibold text-gray-700">Cấu hình #{index + 1}</span>
                            <button
                              type="button"
                              onClick={() => removeCauHinh(index)}
                              className="text-red-600 hover:text-red-800"
                            >
                              <MinusCircle className="w-5 h-5" />
                            </button>
                          </div>

                          <div className="grid grid-cols-2 gap-3">
                            <div>
                              <label className="block text-xs font-medium text-gray-600 mb-1">Cấp độ</label>
                              <select
                                value={cauHinh.capDo}
                                onChange={(e) => updateCauHinh(index, 'capDo', Number(e.target.value))}
                                className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                              >
                                <option value={0}>A1</option>
                                <option value={1}>A2</option>
                                <option value={2}>B1</option>
                                <option value={3}>B2</option>
                                <option value={4}>C1</option>
                                <option value={5}>C2</option>
                              </select>
                            </div>

                            <div>
                              <label className="block text-xs font-medium text-gray-600 mb-1">Độ khó</label>
                              <select
                                value={cauHinh.doKho}
                                onChange={(e) => updateCauHinh(index, 'doKho', Number(e.target.value))}
                                className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                              >
                                <option value={0}>Dễ</option>
                                <option value={1}>Trung bình</option>
                                <option value={2}>Khó</option>
                              </select>
                            </div>

                            <div>
                              <label className="block text-xs font-medium text-gray-600 mb-1">Kỹ năng</label>
                              <select
                                value={cauHinh.kyNang}
                                onChange={(e) => updateCauHinh(index, 'kyNang', Number(e.target.value))}
                                className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                              >
                                <option value={0}>Nghe</option>
                                <option value={1}>Đọc</option>
                                <option value={2}>Ngữ pháp</option>
                                <option value={3}>Từ vựng</option>
                                <option value={4}>Viết</option>
                              </select>
                            </div>

                            <div>
                              <label className="block text-xs font-medium text-gray-600 mb-1">Chế độ câu hỏi</label>
                              <select
                                value={cauHinh.cheDoCauHoi}
                                onChange={(e) => updateCauHinh(index, 'cheDoCauHoi', Number(e.target.value))}
                                className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                              >
                                <option value={0}>Đơn</option>
                                <option value={1}>Nhóm</option>
                              </select>
                            </div>

                            <div className="col-span-2">
                              <label className="block text-xs font-medium text-gray-600 mb-1">Số câu hỏi</label>
                              <input
                                type="number"
                                value={cauHinh.soCauHoi}
                                onChange={(e) => updateCauHinh(index, 'soCauHoi', Number(e.target.value))}
                                className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                                min="1"
                                required
                              />
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}

                {message && messageType === "error" && (
                  <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-lg flex items-start space-x-2">
                    <X className="w-5 h-5 flex-shrink-0 mt-0.5" />
                    <span>{message}</span>
                  </div>
                )}

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingPeriod ? "Cập nhật" : "Thêm mới"}
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
