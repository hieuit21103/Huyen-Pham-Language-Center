import { Database, Download, Upload, HardDrive, Clock, CheckCircle, AlertCircle, Trash2, RefreshCw } from "lucide-react";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { setLightTheme } from "./_layout";
import { getProfile } from "~/apis/Profile";
import { VaiTro } from "~/types/enums";

interface BackupFile {
  id: string;
  tenFile: string;
  dungLuong: number;
  ngayTao: string;
  loai: string;
  moTa?: string;
}

export default function SaoLuuDuLieu() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [isAdmin, setIsAdmin] = useState(false);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error">("success");
  const [backupFiles, setBackupFiles] = useState<BackupFile[]>([]);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showRestoreModal, setShowRestoreModal] = useState(false);
  const [selectedFile, setSelectedFile] = useState<BackupFile | null>(null);
  const [uploadFile, setUploadFile] = useState<File | null>(null);

  // Form data for creating backup
  const [backupForm, setBackupForm] = useState({
    tenFile: "",
    loai: "full", // full, partial
    baoGomBang: {
      hocVien: true,
      giaoVien: true,
      khoaHoc: true,
      lopHoc: true,
      dangKy: true,
      thanhToan: true,
      deThi: true,
      cauHoi: true,
      kyThi: true,
      lichHoc: true,
    },
    moTa: "",
  });

  useEffect(() => {
    setLightTheme();
    checkAdminAccess();
  }, []);

  const checkAdminAccess = async () => {
    const response = await getProfile();
    if (!response.success || !response.data) {
      navigate("/dang-nhap");
      return;
    }
    if (response.data.vaiTro !== VaiTro.Admin) {
      setIsAdmin(false);
      setLoading(false);
      return;
    }
    setIsAdmin(true);
    loadBackupFiles();
  };

  const loadBackupFiles = async () => {
    setLoading(true);
    // TODO: Gọi API để lấy danh sách file backup
    // const response = await getBackupFiles();
    // if (response.success && response.data) {
    //   setBackupFiles(response.data);
    // }
    
    // Mock data for demo
    setTimeout(() => {
      setBackupFiles([
        {
          id: "1",
          tenFile: "backup_full_2024_11_02_14_30.sql",
          dungLuong: 15728640, // 15MB
          ngayTao: "2024-11-02T14:30:00",
          loai: "full",
          moTa: "Sao lưu đầy đủ tất cả dữ liệu",
        },
        {
          id: "2",
          tenFile: "backup_partial_2024_11_01_10_00.sql",
          dungLuong: 5242880, // 5MB
          ngayTao: "2024-11-01T10:00:00",
          loai: "partial",
          moTa: "Sao lưu học viên và đăng ký",
        },
      ]);
      setLoading(false);
    }, 500);
  };

  const handleCreateBackup = async () => {
    setLoading(true);
    
    // TODO: Gọi API để tạo backup
    // const response = await createBackup(backupForm);
    // if (response.success) {
    //   setMessage("Tạo bản sao lưu thành công!");
    //   setMessageType("success");
    //   loadBackupFiles();
    //   setShowCreateModal(false);
    //   resetForm();
    // } else {
    //   setMessage(response.message || "Tạo bản sao lưu thất bại!");
    //   setMessageType("error");
    // }
    
    // Mock success
    setTimeout(() => {
      setLoading(false);
      setMessage("Tạo bản sao lưu thành công!");
      setMessageType("success");
      setShowCreateModal(false);
      resetForm();
      loadBackupFiles();
      setTimeout(() => setMessage(""), 3000);
    }, 2000);
  };

  const handleDownloadBackup = async (file: BackupFile) => {
    // TODO: Gọi API để download backup
    // const response = await downloadBackup(file.id);
    // if (response.success && response.data) {
    //   const blob = response.data as Blob;
    //   const url = window.URL.createObjectURL(blob);
    //   const link = document.createElement('a');
    //   link.href = url;
    //   link.download = file.tenFile;
    //   document.body.appendChild(link);
    //   link.click();
    //   document.body.removeChild(link);
    //   window.URL.revokeObjectURL(url);
    // }
    
    setMessage(`Đang tải xuống ${file.tenFile}...`);
    setMessageType("success");
    setTimeout(() => setMessage(""), 3000);
  };

  const handleRestoreBackup = async () => {
    if (!selectedFile) return;
    
    if (!confirm(`Bạn có chắc chắn muốn khôi phục dữ liệu từ "${selectedFile.tenFile}"? Dữ liệu hiện tại sẽ bị ghi đè!`)) {
      return;
    }

    setLoading(true);
    
    // TODO: Gọi API để restore backup
    // const response = await restoreBackup(selectedFile.id);
    // if (response.success) {
    //   setMessage("Khôi phục dữ liệu thành công!");
    //   setMessageType("success");
    //   setShowRestoreModal(false);
    // } else {
    //   setMessage(response.message || "Khôi phục dữ liệu thất bại!");
    //   setMessageType("error");
    // }
    
    // Mock success
    setTimeout(() => {
      setLoading(false);
      setMessage("Khôi phục dữ liệu thành công!");
      setMessageType("success");
      setShowRestoreModal(false);
      setTimeout(() => setMessage(""), 3000);
    }, 2000);
  };

  const handleUploadBackup = async () => {
    if (!uploadFile) return;

    setLoading(true);
    
    // TODO: Gọi API để upload backup
    // const response = await uploadBackup(uploadFile);
    // if (response.success) {
    //   setMessage("Tải lên bản sao lưu thành công!");
    //   setMessageType("success");
    //   loadBackupFiles();
    // } else {
    //   setMessage(response.message || "Tải lên thất bại!");
    //   setMessageType("error");
    // }
    
    // Mock success
    setTimeout(() => {
      setLoading(false);
      setMessage("Tải lên bản sao lưu thành công!");
      setMessageType("success");
      setUploadFile(null);
      loadBackupFiles();
      setTimeout(() => setMessage(""), 3000);
    }, 2000);
  };

  const handleDeleteBackup = async (file: BackupFile) => {
    if (!confirm(`Bạn có chắc chắn muốn xóa "${file.tenFile}"?`)) {
      return;
    }

    setLoading(true);
    
    // TODO: Gọi API để xóa backup
    // const response = await deleteBackup(file.id);
    // if (response.success) {
    //   setMessage("Xóa bản sao lưu thành công!");
    //   setMessageType("success");
    //   loadBackupFiles();
    // } else {
    //   setMessage(response.message || "Xóa thất bại!");
    //   setMessageType("error");
    // }
    
    // Mock success
    setTimeout(() => {
      setLoading(false);
      setMessage("Xóa bản sao lưu thành công!");
      setMessageType("success");
      loadBackupFiles();
      setTimeout(() => setMessage(""), 3000);
    }, 1000);
  };

  const resetForm = () => {
    setBackupForm({
      tenFile: "",
      loai: "full",
      baoGomBang: {
        hocVien: true,
        giaoVien: true,
        khoaHoc: true,
        lopHoc: true,
        dangKy: true,
        thanhToan: true,
        deThi: true,
        cauHoi: true,
        kyThi: true,
        lichHoc: true,
      },
      moTa: "",
    });
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + " " + sizes[i];
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString("vi-VN");
  };

  const getLoaiText = (loai: string) => {
    return loai === "full" ? "Đầy đủ" : "Từng phần";
  };

  const getLoaiColor = (loai: string) => {
    return loai === "full" ? "bg-blue-100 text-blue-800" : "bg-purple-100 text-purple-800";
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
          <p className="text-gray-600">Đang kiểm tra quyền truy cập...</p>
        </div>
      </div>
    );
  }

  if (!isAdmin) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="bg-white rounded-xl shadow-lg border border-gray-200 p-8 max-w-md w-full text-center">
          <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <AlertCircle className="w-8 h-8 text-red-600" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Truy cập bị từ chối</h2>
          <p className="text-gray-600 mb-6">
            Chỉ Quản trị viên mới có quyền truy cập trang này.
          </p>
          <button
            onClick={() => navigate("/admin")}
            className="w-full bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
          >
            Quay lại Dashboard
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">{message && (
        <div
          className={`${
            messageType === "success"
              ? "bg-green-100 border-green-400 text-green-700"
              : "bg-red-100 border-red-400 text-red-700"
          } border px-4 py-3 rounded-lg flex items-center space-x-2`}
        >
          {messageType === "success" ? (
            <CheckCircle className="w-5 h-5" />
          ) : (
            <AlertCircle className="w-5 h-5" />
          )}
          <span>{message}</span>
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Sao lưu dữ liệu</h1>
          <p className="text-gray-600 mt-1">Quản lý sao lưu và khôi phục dữ liệu hệ thống</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <button
            onClick={() => setShowCreateModal(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
          >
            <Database className="w-5 h-5" />
            <span>Tạo bản sao lưu</span>
          </button>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white border border-blue-200 rounded-xl shadow-sm p-6">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-blue-600 mb-2">Tổng số bản sao lưu</p>
              <p className="text-3xl font-bold text-gray-900">{backupFiles.length}</p>
            </div>
            <div className="bg-blue-50 p-3 rounded-lg">
              <Database className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div className="bg-white border border-green-200 rounded-xl shadow-sm p-6">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-green-600 mb-2">Sao lưu hoàn tất</p>
              <p className="text-3xl font-bold text-gray-900">{backupFiles.filter(f => f.loai === 'full').length}</p>
            </div>
            <div className="bg-green-50 p-3 rounded-lg">
              <CheckCircle className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div className="bg-white border border-purple-200 rounded-xl shadow-sm p-6">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-purple-600 mb-2">Tổng dung lượng</p>
              <p className="text-3xl font-bold text-gray-900">
                {formatFileSize(backupFiles.reduce((sum, f) => sum + f.dungLuong, 0))}
              </p>
            </div>
            <div className="bg-purple-50 p-3 rounded-lg">
              <HardDrive className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </div>

        <div className="bg-white border border-orange-200 rounded-xl shadow-sm p-6">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-orange-600 mb-2">Sao lưu gần nhất</p>
              <p className="text-lg font-bold text-gray-900">
                {backupFiles.length > 0 ? new Date(backupFiles[0].ngayTao).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' }) : "—"}
              </p>
            </div>
            <div className="bg-orange-50 p-3 rounded-lg">
              <Clock className="w-6 h-6 text-orange-600" />
            </div>
          </div>
        </div>
      </div>

      {/* Upload Section */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center space-x-2">
          <Upload className="w-5 h-5" />
          <span>Tải lên bản sao lưu</span>
        </h2>
        <div className="flex items-center space-x-4">
          <input
            type="file"
            accept=".sql,.zip,.backup"
            onChange={(e) => setUploadFile(e.target.files?.[0] || null)}
            className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          />
          <button
            onClick={handleUploadBackup}
            disabled={!uploadFile || loading}
            className="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center space-x-2"
          >
            <Upload className="w-5 h-5" />
            <span>Tải lên</span>
          </button>
        </div>
        {uploadFile && (
          <p className="text-sm text-gray-600 mt-2">
            Đã chọn: {uploadFile.name} ({formatFileSize(uploadFile.size)})
          </p>
        )}
      </div>

      {/* Backup Files List */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        <div className="p-6 border-b border-gray-200 flex justify-between items-center">
          <h2 className="text-lg font-semibold text-gray-900 flex items-center space-x-2">
            <Database className="w-5 h-5" />
            <span>Danh sách bản sao lưu</span>
          </h2>
          <button
            onClick={loadBackupFiles}
            className="text-gray-600 hover:text-gray-900 p-2 hover:bg-gray-100 rounded-lg transition-colors"
            title="Tải lại"
          >
            <RefreshCw className="w-5 h-5" />
          </button>
        </div>

        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : backupFiles.length === 0 ? (
          <div className="p-8 text-center">
            <Database className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Chưa có bản sao lưu nào</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Tên file
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Loại
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Dung lượng
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Ngày tạo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Mô tả
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {backupFiles.map((file) => (
                  <tr key={file.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center space-x-2">
                        <Database className="w-5 h-5 text-gray-400" />
                        <span className="text-sm font-medium text-gray-900">{file.tenFile}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getLoaiColor(file.loai)}`}>
                        {getLoaiText(file.loai)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {formatFileSize(file.dungLuong)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {formatDate(file.ngayTao)}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      <span className="line-clamp-2">{file.moTa || "—"}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end space-x-2">
                        <button
                          onClick={() => handleDownloadBackup(file)}
                          className="text-blue-600 hover:text-blue-900 p-2 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Tải xuống"
                        >
                          <Download className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => {
                            setSelectedFile(file);
                            setShowRestoreModal(true);
                          }}
                          className="text-green-600 hover:text-green-900 p-2 hover:bg-green-50 rounded-lg transition-colors"
                          title="Khôi phục"
                        >
                          <RefreshCw className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDeleteBackup(file)}
                          className="text-red-600 hover:text-red-900 p-2 hover:bg-red-50 rounded-lg transition-colors"
                          title="Xóa"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Create Backup Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">Tạo bản sao lưu</h2>
                <button
                  onClick={() => setShowCreateModal(false)}
                  className="text-gray-500 hover:text-gray-700"
                >
                  <AlertCircle className="w-6 h-6" />
                </button>
              </div>

              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Tên file (tự động tạo nếu để trống)
                  </label>
                  <input
                    type="text"
                    value={backupForm.tenFile}
                    onChange={(e) => setBackupForm({ ...backupForm, tenFile: e.target.value })}
                    placeholder="backup_2024_11_02.sql"
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Loại sao lưu</label>
                  <select
                    value={backupForm.loai}
                    onChange={(e) => setBackupForm({ ...backupForm, loai: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                    <option value="full">Đầy đủ (tất cả dữ liệu)</option>
                    <option value="partial">Từng phần (chọn bảng)</option>
                  </select>
                </div>

                {backupForm.loai === "partial" && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Chọn bảng dữ liệu
                    </label>
                    <div className="grid grid-cols-2 gap-3 p-4 bg-gray-50 rounded-lg">
                      {Object.entries(backupForm.baoGomBang).map(([key, value]) => (
                        <label key={key} className="flex items-center space-x-2 cursor-pointer">
                          <input
                            type="checkbox"
                            checked={value}
                            onChange={(e) =>
                              setBackupForm({
                                ...backupForm,
                                baoGomBang: {
                                  ...backupForm.baoGomBang,
                                  [key]: e.target.checked,
                                },
                              })
                            }
                            className="rounded border-gray-300 text-gray-900 focus:ring-gray-900"
                          />
                          <span className="text-sm text-gray-700 capitalize">
                            {key.replace(/([A-Z])/g, " $1").trim()}
                          </span>
                        </label>
                      ))}
                    </div>
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Mô tả</label>
                  <textarea
                    value={backupForm.moTa}
                    onChange={(e) => setBackupForm({ ...backupForm, moTa: e.target.value })}
                    rows={3}
                    placeholder="Mô tả ngắn gọn về bản sao lưu này..."
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    onClick={handleCreateBackup}
                    disabled={loading}
                    className="flex-1 bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center space-x-2"
                  >
                    {loading ? (
                      <>
                        <RefreshCw className="w-5 h-5 animate-spin" />
                        <span>Đang xử lý...</span>
                      </>
                    ) : (
                      <>
                        <Database className="w-5 h-5" />
                        <span>Tạo sao lưu</span>
                      </>
                    )}
                  </button>
                  <button
                    onClick={() => setShowCreateModal(false)}
                    className="flex-1 bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors"
                  >
                    Hủy
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Restore Backup Modal */}
      {showRestoreModal && selectedFile && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">Khôi phục dữ liệu</h2>
                <button
                  onClick={() => setShowRestoreModal(false)}
                  className="text-gray-500 hover:text-gray-700"
                >
                  <AlertCircle className="w-6 h-6" />
                </button>
              </div>

              <div className="space-y-4">
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
                  <div className="flex items-start space-x-3">
                    <AlertCircle className="w-5 h-5 text-yellow-600 mt-0.5" />
                    <div>
                      <p className="text-sm font-medium text-yellow-800">Cảnh báo!</p>
                      <p className="text-sm text-yellow-700 mt-1">
                        Thao tác này sẽ ghi đè toàn bộ dữ liệu hiện tại. Vui lòng đảm bảo bạn đã
                        sao lưu dữ liệu quan trọng trước khi thực hiện.
                      </p>
                    </div>
                  </div>
                </div>

                <div className="bg-gray-50 rounded-lg p-4">
                  <p className="text-sm text-gray-700">
                    <strong>File:</strong> {selectedFile.tenFile}
                  </p>
                  <p className="text-sm text-gray-700 mt-1">
                    <strong>Dung lượng:</strong> {formatFileSize(selectedFile.dungLuong)}
                  </p>
                  <p className="text-sm text-gray-700 mt-1">
                    <strong>Ngày tạo:</strong> {formatDate(selectedFile.ngayTao)}
                  </p>
                  {selectedFile.moTa && (
                    <p className="text-sm text-gray-700 mt-1">
                      <strong>Mô tả:</strong> {selectedFile.moTa}
                    </p>
                  )}
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    onClick={handleRestoreBackup}
                    disabled={loading}
                    className="flex-1 bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center space-x-2"
                  >
                    {loading ? (
                      <>
                        <RefreshCw className="w-5 h-5 animate-spin" />
                        <span>Đang khôi phục...</span>
                      </>
                    ) : (
                      <>
                        <RefreshCw className="w-5 h-5" />
                        <span>Xác nhận khôi phục</span>
                      </>
                    )}
                  </button>
                  <button
                    onClick={() => setShowRestoreModal(false)}
                    className="flex-1 bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors"
                  >
                    Hủy
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
