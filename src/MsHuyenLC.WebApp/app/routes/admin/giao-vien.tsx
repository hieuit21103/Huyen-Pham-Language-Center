import { Search, Plus, Edit, Trash2, User, X } from "lucide-react";
import { useState, useEffect } from "react";
import { getGiaoViens, createGiaoVien, updateGiaoVien, deleteGiaoVien } from "~/apis/GiaoVien";
import { getTaiKhoans } from "~/apis/TaiKhoan";
import type { GiaoVien } from "~/types/index";
import { setLightTheme } from "./_layout";

export default function AdminTeachers() {
  const [searchTerm, setSearchTerm] = useState("");
  const [teachers, setTeachers] = useState<GiaoVien[]>([]);
  const [accounts, setAccounts] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingTeacher, setEditingTeacher] = useState<GiaoVien | null>(null);
  const [message, setMessage] = useState("");
  
  const [formData, setFormData] = useState({
    hoTen: "",
    chuyenMon: "",
    trinhDo: "",
    kinhNghiem: 0,
    taiKhoanId: "",
  });

  useEffect(() => {
    setLightTheme();
    loadTeachers();
    loadAccounts();
  }, []);

  const loadTeachers = async () => {
    setLoading(true);
    const response = await getGiaoViens();
    if (response.success && response.data) {
      setTeachers(response.data);
    }
    setLoading(false);
  };

  const loadAccounts = async () => {
    const response = await getTaiKhoans();
    if (response.success && response.data) {
      setAccounts(response.data);
    }
  };

  const handleCreate = () => {
    setEditingTeacher(null);
    setFormData({
      hoTen: "",
      chuyenMon: "",
      trinhDo: "",
      kinhNghiem: 0,
      taiKhoanId: "",
    });
    setShowModal(true);
  };

  const handleEdit = (teacher: GiaoVien) => {
    setEditingTeacher(teacher);
    setFormData({
      hoTen: teacher.hoTen || "",
      chuyenMon: teacher.chuyenMon || "",
      trinhDo: teacher.trinhDo || "",
      kinhNghiem: teacher.kinhNghiem || 0,
      taiKhoanId: teacher.taiKhoanId || "",
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const requestData = {
      ...formData,
      kinhNghiem: formData.kinhNghiem.toString(),
    };
    
    if (editingTeacher) {
      const response = await updateGiaoVien(editingTeacher.id!, requestData);
      setMessage(response.message || "");
      if (response.success) {
        loadTeachers();
        setShowModal(false);
      }
    } else {
      const response = await createGiaoVien(requestData);
      setMessage(response.message || "");
      if (response.success) {
        loadTeachers();
        setShowModal(false);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa giáo viên này?")) {
      const response = await deleteGiaoVien(id);
      setMessage(response.message || "");
      if (response.success) {
        loadTeachers();
      }
    }
  };

  const filteredTeachers = teachers.filter(teacher =>
    teacher.hoTen?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    teacher.chuyenMon?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {message && (
        <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg">
          {message}
        </div>
      )}

      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý giáo viên</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả giáo viên</p>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm giáo viên</span>
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm giáo viên..."
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
            {filteredTeachers.map((teacher) => (
            <div key={teacher.id} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              <div className="p-6">
                <div className="flex items-start justify-between mb-4">
                  <div className="w-12 h-12 bg-gray-900 rounded-lg flex items-center justify-center">
                    <User className="w-6 h-6 text-white" />
                  </div>
                </div>
                
                <h3 className="text-xl font-bold text-gray-900 mb-2">{teacher.hoTen}</h3>
                <p className="text-sm text-gray-600 mb-4">{teacher.chuyenMon}</p>
                
                <div className="space-y-2 mb-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Trình độ:</span>
                    <span className="font-semibold text-gray-900">{teacher.trinhDo}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Kinh nghiệm:</span>
                    <span className="font-semibold text-gray-900">{teacher.kinhNghiem || 0} năm</span>
                  </div>
                </div>
                
                <div className="flex space-x-2 pt-4 border-t border-gray-200">
                  <button 
                    onClick={() => handleEdit(teacher)}
                    className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center space-x-2"
                  >
                    <Edit className="w-4 h-4" />
                    <span>Sửa</span>
                  </button>
                  <button 
                    onClick={() => handleDelete(teacher.id!)}
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
                  {editingTeacher ? "Chỉnh sửa giáo viên" : "Thêm giáo viên mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Họ tên</label>
                  <input
                    type="text"
                    value={formData.hoTen}
                    onChange={(e) => setFormData({ ...formData, hoTen: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Chuyên môn</label>
                  <input
                    type="text"
                    value={formData.chuyenMon}
                    onChange={(e) => setFormData({ ...formData, chuyenMon: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Trình độ</label>
                  <input
                    type="text"
                    value={formData.trinhDo}
                    onChange={(e) => setFormData({ ...formData, trinhDo: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Kinh nghiệm (năm)</label>
                  <input
                    type="number"
                    value={formData.kinhNghiem}
                    onChange={(e) => setFormData({ ...formData, kinhNghiem: Number(e.target.value) })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                    min="0"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tài khoản</label>
                  <select
                    value={formData.taiKhoanId}
                    onChange={(e) => setFormData({ ...formData, taiKhoanId: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                    <option value="">-- Chọn tài khoản (tùy chọn) --</option>
                    {accounts.filter(acc => acc.vaiTro === 2).map((account) => (
                      <option key={account.id} value={account.id}>
                        {account.tenDangNhap}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingTeacher ? "Cập nhật" : "Thêm mới"}
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
