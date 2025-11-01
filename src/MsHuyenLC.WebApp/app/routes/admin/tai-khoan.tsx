import { Search, Plus, Edit, Trash2, User, Lock, Mail, Phone, X, Eye, EyeOff } from "lucide-react";
import { useState, useEffect } from "react";
import { getTaiKhoans, createTaiKhoan, updateTaiKhoan, deleteTaiKhoan } from "~/apis/TaiKhoan";
import type { TaiKhoan, TaiKhoanRequest, TaiKhoanUpdateRequest } from "~/types/auth.types";
import { VaiTro, TrangThaiTaiKhoan } from "~/types/enums";
import { setLightTheme } from "./_layout";

export default function AdminAccounts() {
  const [searchTerm, setSearchTerm] = useState("");
  const [accounts, setAccounts] = useState<TaiKhoan[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingAccount, setEditingAccount] = useState<TaiKhoan | null>(null);
  const [message, setMessage] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  
  const [formData, setFormData] = useState<TaiKhoanRequest & { trangThai?: TrangThaiTaiKhoan }>({
    tenDangNhap: "",
    matKhau: "",
    email: "",
    sdt: "",
    vaiTro: VaiTro.HocVien,
    trangThai: TrangThaiTaiKhoan.HoatDong,
  });

  useEffect(() => {
    setLightTheme();
    loadAccounts();
  }, []);

  const loadAccounts = async () => {
    setLoading(true);
    const response = await getTaiKhoans({ 
      sortBy: "tenDangNhap",
      sortOrder: "asc"
    });
    if (response.success && response.data) {
      setAccounts(response.data);
    }
    setLoading(false);
  };

  const handleCreate = () => {
    setEditingAccount(null);
    setFormData({
      tenDangNhap: "",
      matKhau: "",
      email: "",
      sdt: "",
      vaiTro: VaiTro.HocVien,
      trangThai: TrangThaiTaiKhoan.HoatDong,
    });
    setShowModal(true);
    setMessage("");
  };

  const handleEdit = (account: TaiKhoan) => {
    setEditingAccount(account);
    setFormData({
      tenDangNhap: account.tenDangNhap || "",
      email: account.email || "",
      sdt: account.sdt || "",
      vaiTro: account.vaiTro || VaiTro.HocVien,
      trangThai: account.trangThai || TrangThaiTaiKhoan.HoatDong,
    });
    setShowModal(true);
    setMessage("");
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (editingAccount) {
      const updateData: TaiKhoanUpdateRequest = {
        email: formData.email,
        sdt: formData.sdt,
        vaiTro: formData.vaiTro,
        trangThai: formData.trangThai,
      };
      const response = await updateTaiKhoan(editingAccount.id!, updateData);
      setMessage(response.message || "");
      if (response.success) {
        loadAccounts();
        setTimeout(() => setShowModal(false), 1500);
      }
    } else {
      const response = await createTaiKhoan(formData);
      setMessage(response.message || "");
      if (response.success) {
        loadAccounts();
        setTimeout(() => setShowModal(false), 1500);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa tài khoản này?")) {
      const response = await deleteTaiKhoan(id);
      setMessage(response.message || "");
      if (response.success) {
        loadAccounts();
      }
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

  const getVaiTroColor = (vaiTro?: VaiTro) => {
    switch (vaiTro) {
      case VaiTro.Admin: return "bg-purple-100 text-purple-800";
      case VaiTro.GiaoVu: return "bg-blue-100 text-blue-800";
      case VaiTro.GiaoVien: return "bg-green-100 text-green-800";
      case VaiTro.HocVien: return "bg-gray-100 text-gray-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const getTrangThaiText = (trangThai?: TrangThaiTaiKhoan) => {
    switch (trangThai) {
      case TrangThaiTaiKhoan.HoatDong: return "Hoạt động";
      case TrangThaiTaiKhoan.TamDung: return "Tạm dừng";
      case TrangThaiTaiKhoan.BiKhoa: return "Bị khóa";
      default: return "—";
    }
  };

  const getTrangThaiColor = (trangThai?: TrangThaiTaiKhoan) => {
    switch (trangThai) {
      case TrangThaiTaiKhoan.HoatDong: return "bg-green-100 text-green-800";
      case TrangThaiTaiKhoan.TamDung: return "bg-yellow-100 text-yellow-800";
      case TrangThaiTaiKhoan.BiKhoa: return "bg-red-100 text-red-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const filteredAccounts = accounts.filter(account =>
    account.tenDangNhap?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    account.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    account.sdt?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {message && (
        <div className={`px-4 py-3 rounded-lg ${
          message.includes("thành công") 
            ? "bg-green-100 border border-green-400 text-green-700"
            : "bg-red-100 border border-red-400 text-red-700"
        }`}>
          {message}
        </div>
      )}

      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý tài khoản</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả tài khoản trong hệ thống</p>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm tài khoản</span>
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm theo tên đăng nhập, email hoặc số điện thoại..."
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
                    Tên đăng nhập
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Email
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Số điện thoại
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Vai trò
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-4 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredAccounts.map((account) => (
                  <tr key={account.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <User className="w-5 h-5 text-gray-400 mr-2" />
                        <span className="font-medium text-gray-900">
                          {account.tenDangNhap || "—"}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <Mail className="w-5 h-5 text-gray-400 mr-2" />
                        <span className="text-gray-700">
                          {account.email || "—"}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <Phone className="w-5 h-5 text-gray-400 mr-2" />
                        <span className="text-gray-700">
                          {account.sdt || "—"}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`px-3 py-1 text-xs font-semibold rounded-full ${getVaiTroColor(account.vaiTro)}`}>
                        {getVaiTroText(account.vaiTro)}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`px-3 py-1 text-xs font-semibold rounded-full ${getTrangThaiColor(account.trangThai)}`}>
                        {getTrangThaiText(account.trangThai)}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex justify-end space-x-2">
                        <button 
                          onClick={() => handleEdit(account)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Chỉnh sửa"
                        >
                          <Edit className="w-5 h-5" />
                        </button>
                        <button 
                          onClick={() => handleDelete(account.id!)}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                          title="Xóa"
                        >
                          <Trash2 className="w-5 h-5" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
                {filteredAccounts.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-gray-500">
                      <User className="w-12 h-12 mx-auto mb-3 text-gray-400" />
                      <p>Không tìm thấy tài khoản nào</p>
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
                  {editingAccount ? "Chỉnh sửa tài khoản" : "Thêm tài khoản mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Tên đăng nhập <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text"
                    value={formData.tenDangNhap}
                    onChange={(e) => setFormData({ ...formData, tenDangNhap: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                    disabled={!!editingAccount}
                  />
                </div>

                {!editingAccount && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Mật khẩu <span className="text-red-500">*</span>
                    </label>
                    <div className="relative">
                      <input
                        type={showPassword ? "text" : "password"}
                        value={formData.matKhau}
                        onChange={(e) => setFormData({ ...formData, matKhau: e.target.value })}
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                        required
                      />
                      <button
                        type="button"
                        onClick={() => setShowPassword(!showPassword)}
                        className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-gray-700"
                      >
                        {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                      </button>
                    </div>
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Email <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Số điện thoại
                  </label>
                  <input
                    type="tel"
                    value={formData.sdt}
                    onChange={(e) => setFormData({ ...formData, sdt: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Vai trò <span className="text-red-500">*</span>
                  </label>
                  <select
                    value={formData.vaiTro}
                    onChange={(e) => setFormData({ ...formData, vaiTro: Number(e.target.value) as VaiTro })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  >
                    <option value={VaiTro.HocVien}>Học viên</option>
                    <option value={VaiTro.GiaoVien}>Giáo viên</option>
                    <option value={VaiTro.GiaoVu}>Giáo vụ</option>
                    <option value={VaiTro.Admin}>Quản trị viên</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Trạng thái <span className="text-red-500">*</span>
                  </label>
                  <select
                    value={formData.trangThai}
                    onChange={(e) => setFormData({ ...formData, trangThai: Number(e.target.value) as TrangThaiTaiKhoan })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  >
                    <option value={TrangThaiTaiKhoan.HoatDong}>Hoạt động</option>
                    <option value={TrangThaiTaiKhoan.TamDung}>Tạm dừng</option>
                    <option value={TrangThaiTaiKhoan.BiKhoa}>Bị khóa</option>
                  </select>
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingAccount ? "Cập nhật" : "Thêm mới"}
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
