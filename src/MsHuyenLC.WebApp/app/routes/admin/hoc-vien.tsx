import { Search, Plus, Edit, Trash2, User, X } from "lucide-react";
import { useState, useEffect } from "react";
import { getHocViens, updateHocVien, deleteHocVien } from "~/apis/HocVien";
import { getTaiKhoans } from "~/apis/TaiKhoan";
import type { GioiTinh, TaiKhoan, HocVien } from "~/types/index";
import { formatDateForInput, formatDateForDisplay } from "~/utils/date-utils";
import { setLightTheme } from "./_layout";

export default function AdminStudents() {
  const [searchTerm, setSearchTerm] = useState("");
  const [students, setStudents] = useState<HocVien[]>([]);
  const [accounts, setAccounts] = useState<TaiKhoan[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingStudent, setEditingStudent] = useState<HocVien | null>(null);
  const [message, setMessage] = useState("");
  
  const [formData, setFormData] = useState({
    hoTen: "",
    ngaySinh: "",
    gioiTinh: 0 as GioiTinh,
    diaChi: "",
  });

  useEffect(() => {
    setLightTheme();
    loadStudents();
    loadAccounts();
  }, []);

  const loadStudents = async () => {
    setLoading(true);
    const response = await getHocViens({ 
      sortBy: 'ngayDangKy',
      sortOrder: 'desc'
    });
    if (response.success && response.data) {
      setStudents(response.data);
    }
    setLoading(false);
  };

  const loadAccounts = async () => {
    const response = await getTaiKhoans({ pageNumber: 1, pageSize: 1000 });
    if (response.success && response.data) {
      setAccounts(response.data);
    }
  };

  const handleEdit = (student: HocVien) => {
    setEditingStudent(student);
    setFormData({
      hoTen: student.hoTen || "",
      ngaySinh: formatDateForInput(student.ngaySinh),
      gioiTinh: student.gioiTinh ?? 0,
      diaChi: student.diaChi || "",
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (editingStudent) {
      const response = await updateHocVien(editingStudent.id!, formData);
      setMessage(response.message || "");
      if (response.success) {
        loadStudents();
        setShowModal(false);
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa học viên này?")) {
      const response = await deleteHocVien(id);
      setMessage(response.message || "");
      if (response.success) {
        loadStudents();
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const filteredStudents = students.filter(student =>
    student.hoTen?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getGenderText = (gender?: GioiTinh) => {
    return gender === 0 ? "Nam" : "Nữ";
  };

  return (
    <div className="space-y-6">
      {message && (
        <div className={`${message.includes("thất bại") || message.includes("Lỗi") ? "bg-red-100 border-red-400 text-red-700" : "bg-green-100 border-green-400 text-green-700"} border px-4 py-3 rounded-lg`}>
          {message}
        </div>
      )}

      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý học viên</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả học viên</p>
        </div>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm học viên theo tên, email, số điện thoại..."
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

      {!loading && filteredStudents.length === 0 && (
        <div className="text-center py-12 bg-white rounded-xl shadow-sm border border-gray-200">
          <User className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-600 text-lg">Không tìm thấy học viên nào</p>
        </div>
      )}

      {!loading && filteredStudents.length > 0 && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Họ tên
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Giới tính
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Ngày sinh
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Liên hệ
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Địa chỉ
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredStudents.map((student) => (
                  <tr key={student.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="w-10 h-10 bg-gray-900 rounded-full flex items-center justify-center mr-3">
                          <User className="w-5 h-5 text-white" />
                        </div>
                        <div className="font-medium text-gray-900">{student.hoTen}</div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {getGenderText(student.gioiTinh)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {formatDateForDisplay(student.ngaySinh)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {accounts.find(account => account.id === student.taiKhoanId)?.email || '—'}<br />
                      {accounts.find(account => account.id === student.taiKhoanId)?.sdt || '—'}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600 max-w-xs truncate">
                      {student.diaChi || '—'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <div className="flex space-x-2">
                        <button 
                          onClick={() => handleEdit(student)}
                          className="text-blue-600 hover:text-blue-900"
                          title="Chỉnh sửa"
                        >
                          <Edit className="w-5 h-5" />
                        </button>
                        <button 
                          onClick={() => handleDelete(student.id!)}
                          className="text-red-600 hover:text-red-900"
                          title="Xóa"
                        >
                          <Trash2 className="w-5 h-5" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
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
                  Chỉnh sửa thông tin học viên
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Họ tên *</label>
                  <input
                    type="text"
                    value={formData.hoTen}
                    onChange={(e) => setFormData({ ...formData, hoTen: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Ngày sinh</label>
                    <input
                      type="date"
                      value={formData.ngaySinh}
                      onChange={(e) => setFormData({ ...formData, ngaySinh: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Giới tính</label>
                    <select
                      value={formData.gioiTinh}
                      onChange={(e) => setFormData({ ...formData, gioiTinh: Number(e.target.value) as GioiTinh })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value={0}>Nam</option>
                      <option value={1}>Nữ</option>
                    </select>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Địa chỉ</label>
                  <textarea
                    value={formData.diaChi}
                    onChange={(e) => setFormData({ ...formData, diaChi: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    rows={3}
                    placeholder="Nhập địa chỉ..."
                  />
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    Cập nhật
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
