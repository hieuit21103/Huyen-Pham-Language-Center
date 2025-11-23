import { Database, Download, Upload, HardDrive, Clock, CheckCircle, AlertCircle, Trash2, RefreshCw } from "lucide-react";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { setLightTheme } from "./_layout";
import { getProfile } from "~/apis/Profile";
import { VaiTro } from "~/types/enums";
import { getAllBackups, createBackup, restoreBackup, deleteBackup } from "~/apis/Backup";

interface BackupFile {
  id: string;
  ngaySaoLuu: string;
  duongDan: string;
}

export default function SaoLuuDuLieu() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [isAdmin, setIsAdmin] = useState(false);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error">("success");
  const [backupFiles, setBackupFiles] = useState<BackupFile[]>([]);
  const [showRestoreModal, setShowRestoreModal] = useState(false);
  const [selectedFile, setSelectedFile] = useState<BackupFile | null>(null);

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
    try {
      const response = await getAllBackups();
      if (response.success && response.data) {
        setBackupFiles(response.data);
      } else {
        showMessage(response.message || "Không thể tải danh sách backup", "error");
      }
    } catch (error: any) {
      showMessage(error.message || "Đã xảy ra lỗi khi tải danh sách backup", "error");
    } finally {
      setLoading(false);
    }
  };

  const showMessage = (msg: string, type: "success" | "error") => {
    setMessage(msg);
    setMessageType(type);
    setTimeout(() => setMessage(""), 3000);
  };

  const handleCreateBackup = async () => {
    if (!confirm("Bạn có chắc chắn muốn tạo bản sao lưu toàn bộ dữ liệu không?")) {
      return;
    }

    setLoading(true);
    try {
      const response = await createBackup();
      if (response.success) {
        showMessage("Tạo bản sao lưu thành công!", "success");
        loadBackupFiles();
      } else {
        showMessage(response.message || "Tạo bản sao lưu thất bại!", "error");
      }
    } catch (error: any) {
      showMessage(error.message || "Đã xảy ra lỗi khi tạo backup", "error");
    } finally {
      setLoading(false);
    }
  };

  const handleDownloadBackup = async (file: BackupFile) => {
    try {
      const fileName = file.duongDan.split('/').pop() || 'backup.sql';
      window.open(file.duongDan, '_blank');
      showMessage(`Đang tải xuống ${fileName}...`, "success");
    } catch (error: any) {
      showMessage(error.message || "Tải xuống thất bại!", "error");
    }
  };

  const handleRestoreBackup = async () => {
    if (!selectedFile) return;
    
    const fileName = getFileName(selectedFile.duongDan);
    if (!confirm(`Bạn có chắc chắn muốn khôi phục dữ liệu từ "${fileName}"? Dữ liệu hiện tại sẽ bị ghi đè!`)) {
      return;
    }

    setLoading(true);
    try {
      const response = await restoreBackup(selectedFile.id);
      if (response.success) {
        showMessage("Khôi phục dữ liệu thành công!", "success");
        setShowRestoreModal(false);
      } else {
        showMessage(response.message || "Khôi phục dữ liệu thất bại!", "error");
      }
    } catch (error: any) {
      showMessage(error.message || "Đã xảy ra lỗi khi khôi phục", "error");
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteBackup = async (file: BackupFile) => {
    const fileName = file.duongDan.split('/').pop() || 'file này';
    if (!confirm(`Bạn có chắc chắn muốn xóa "${fileName}"?`)) {
      return;
    }

    setLoading(true);
    try {
      const response = await deleteBackup(file.id);
      if (response.success) {
        showMessage("Xóa bản sao lưu thành công!", "success");
        loadBackupFiles();
      } else {
        showMessage(response.message || "Xóa thất bại!", "error");
      }
    } catch (error: any) {
      showMessage(error.message || "Đã xảy ra lỗi khi xóa backup", "error");
    } finally {
      setLoading(false);
    }
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

  const getFileName = (path: string) => {
    return path.split('/').pop() || 'backup.sql';
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
        <button
          onClick={handleCreateBackup}
          disabled={loading}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2 disabled:bg-gray-400 disabled:cursor-not-allowed"
        >
          <Database className="w-5 h-5" />
          <span>Tạo bản sao lưu</span>
        </button>
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
                    Ngày sao lưu
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Đường dẫn
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
                        <span className="text-sm font-medium text-gray-900">{getFileName(file.duongDan)}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {formatDate(file.ngaySaoLuu)}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      <span className="truncate max-w-xs block" title={file.duongDan}>{file.duongDan}</span>
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
                    <strong>File:</strong> {getFileName(selectedFile.duongDan)}
                  </p>
                  <p className="text-sm text-gray-700 mt-1">
                    <strong>Ngày sao lưu:</strong> {formatDate(selectedFile.ngaySaoLuu)}
                  </p>
                  <p className="text-sm text-gray-700 mt-1">
                    <strong>Đường dẫn:</strong> <span className="break-all">{selectedFile.duongDan}</span>
                  </p>
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
