import { Search, MessageSquare, Eye, Trash2, ChevronLeft, ChevronRight, X, Calendar, User } from "lucide-react";
import { useState, useEffect } from "react";
import { getPhanHois, deletePhanHoi, type PhanHoiResponse } from "~/apis/PhanHoi";
import { formatDateTime } from "~/utils/date-utils";
import { setLightTheme } from "./_layout";

export default function AdminFeedback() {
  const [searchTerm, setSearchTerm] = useState("");
  const [feedbacks, setFeedbacks] = useState<PhanHoiResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");
  const [selectedFeedback, setSelectedFeedback] = useState<PhanHoiResponse | null>(null);
  const [showDetailModal, setShowDetailModal] = useState(false);
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10);

  useEffect(() => {
    setLightTheme();
    loadFeedbacks();
  }, []);

  const loadFeedbacks = async () => {
    setLoading(true);
    const response = await getPhanHois();
    if (response.success && response.data) {
      setFeedbacks(response.data);
    }
    setLoading(false);
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa phản hồi này?")) {
      const response = await deletePhanHoi(id);
      setMessage(response.message || "");
      if (response.success) {
        loadFeedbacks();
      }
    }
  };

  const handleViewDetail = (feedback: PhanHoiResponse) => {
    setSelectedFeedback(feedback);
    setShowDetailModal(true);
  };

  const getLoaiPhanHoiColor = (loai?: string) => {
    switch (loai) {
      case "GopY":
        return "bg-blue-100 text-blue-800";
      case "KhieuNai":
        return "bg-red-100 text-red-800";
      case "HoTro":
        return "bg-yellow-100 text-yellow-800";
      case "Khac":
        return "bg-gray-100 text-gray-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const getLoaiPhanHoiText = (loai?: string) => {
    switch (loai) {
      case "general":
        return "Góp ý";
      case "KhieuNai":
        return "Khiếu nại";
      case "HoTro":
        return "Hỗ trợ";
      case "Khac":
        return "Khác";
      default:
        return "Không xác định";
    }
  };

  const filteredFeedbacks = feedbacks.filter(feedback =>
    feedback.tieuDe?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    feedback.noiDung?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    feedback.hocVien?.hoTen?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentItems = filteredFeedbacks.slice(indexOfFirstItem, indexOfLastItem);
  const totalPages = Math.ceil(filteredFeedbacks.length / itemsPerPage);

  const paginate = (pageNumber: number) => setCurrentPage(pageNumber);

  return (
    <div className="space-y-6">
      {/* Message */}
      {message && (
        <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg">
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý phản hồi</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả phản hồi từ học viên</p>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Tìm kiếm phản hồi..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setCurrentPage(1);
              }}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>
        </div>
      </div>

      {/* Loading */}
      {loading && (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
        </div>
      )}

      {/* Feedback Table */}
      {!loading && (
        <>
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
            {currentItems.length === 0 ? (
              <div className="text-center py-12">
                <MessageSquare className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                <p className="text-gray-600 text-lg">Không có phản hồi nào</p>
              </div>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-gray-50 border-b border-gray-200">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                        STT
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                        Học viên
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                        Loại
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                        Tiêu đề
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                        Nội dung
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                        Ngày tạo
                      </th>
                      <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                        Thao tác
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {currentItems.map((feedback, index) => (
                      <tr key={feedback.id} className="hover:bg-gray-50 transition-colors">
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {indexOfFirstItem + index + 1}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="flex items-center">
                            <User className="w-4 h-4 mr-2 text-gray-400" />
                            <span className="text-sm font-medium text-gray-900">
                              {feedback.hocVien?.hoTen || "—"}
                            </span>
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold ${getLoaiPhanHoiColor(feedback.loaiPhanHoi)}`}>
                            {getLoaiPhanHoiText(feedback.loaiPhanHoi)}
                          </span>
                        </td>
                        <td className="px-6 py-4">
                          <div className="text-sm font-medium text-gray-900 max-w-xs truncate">
                            {feedback.tieuDe || "—"}
                          </div>
                        </td>
                        <td className="px-6 py-4">
                          <div className="text-sm text-gray-600 max-w-md truncate">
                            {feedback.noiDung || "—"}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="flex items-center text-sm text-gray-600">
                            <Calendar className="w-4 h-4 mr-2 text-gray-400" />
                            {feedback.ngayTao ?? "—"}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-center">
                          <div className="flex items-center justify-center space-x-2">
                            <button
                              onClick={() => handleViewDetail(feedback)}
                              className="bg-blue-600 text-white px-3 py-1 rounded-lg hover:bg-blue-700 transition-colors text-sm inline-flex items-center space-x-1"
                            >
                              <Eye className="w-4 h-4" />
                              <span>Xem</span>
                            </button>
                            <button
                              onClick={() => handleDelete(feedback.id!)}
                              className="bg-red-600 text-white px-3 py-1 rounded-lg hover:bg-red-700 transition-colors text-sm inline-flex items-center space-x-1"
                            >
                              <Trash2 className="w-4 h-4" />
                              <span>Xóa</span>
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

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-between bg-white rounded-xl shadow-sm border border-gray-200 px-6 py-4">
              <div className="text-sm text-gray-600">
                Hiển thị {indexOfFirstItem + 1} - {Math.min(indexOfLastItem, filteredFeedbacks.length)} trong tổng số {filteredFeedbacks.length} phản hồi
              </div>
              <div className="flex items-center space-x-2">
                <button
                  onClick={() => paginate(currentPage - 1)}
                  disabled={currentPage === 1}
                  className={`p-2 rounded-lg ${
                    currentPage === 1
                      ? "bg-gray-100 text-gray-400 cursor-not-allowed"
                      : "bg-gray-900 text-white hover:bg-gray-800"
                  } transition-colors`}
                >
                  <ChevronLeft className="w-5 h-5" />
                </button>
                
                <div className="flex items-center space-x-1">
                  {[...Array(totalPages)].map((_, index) => {
                    const pageNumber = index + 1;
                    if (
                      pageNumber === 1 ||
                      pageNumber === totalPages ||
                      (pageNumber >= currentPage - 1 && pageNumber <= currentPage + 1)
                    ) {
                      return (
                        <button
                          key={pageNumber}
                          onClick={() => paginate(pageNumber)}
                          className={`px-4 py-2 rounded-lg ${
                            currentPage === pageNumber
                              ? "bg-gray-900 text-white"
                              : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                          } transition-colors`}
                        >
                          {pageNumber}
                        </button>
                      );
                    } else if (
                      pageNumber === currentPage - 2 ||
                      pageNumber === currentPage + 2
                    ) {
                      return <span key={pageNumber} className="px-2">...</span>;
                    }
                    return null;
                  })}
                </div>

                <button
                  onClick={() => paginate(currentPage + 1)}
                  disabled={currentPage === totalPages}
                  className={`p-2 rounded-lg ${
                    currentPage === totalPages
                      ? "bg-gray-100 text-gray-400 cursor-not-allowed"
                      : "bg-gray-900 text-white hover:bg-gray-800"
                  } transition-colors`}
                >
                  <ChevronRight className="w-5 h-5" />
                </button>
              </div>
            </div>
          )}
        </>
      )}

      {/* Detail Modal */}
      {showDetailModal && selectedFeedback && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white p-6 border-b border-gray-200 flex justify-between items-center z-10">
              <h2 className="text-2xl font-bold text-gray-900">Chi tiết phản hồi</h2>
              <button
                onClick={() => setShowDetailModal(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            <div className="p-6 space-y-6">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-600">Học viên</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedFeedback.hocVien?.hoTen || "—"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Loại phản hồi</p>
                  <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-semibold ${getLoaiPhanHoiColor(selectedFeedback.loaiPhanHoi)}`}>
                    {getLoaiPhanHoiText(selectedFeedback.loaiPhanHoi)}
                  </span>
                </div>
                <div className="col-span-2">
                  <p className="text-sm text-gray-600">Ngày gửi</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedFeedback.ngayTao ? formatDateTime(selectedFeedback.ngayTao) : "—"}
                  </p>
                </div>
              </div>

              <div>
                <p className="text-sm text-gray-600 mb-2">Tiêu đề</p>
                <p className="text-lg font-semibold text-gray-900">
                  {selectedFeedback.tieuDe || "—"}
                </p>
              </div>

              <div>
                <p className="text-sm text-gray-600 mb-2">Nội dung</p>
                <div className="bg-gray-50 rounded-lg p-4 border border-gray-200">
                  <p className="text-base text-gray-900 whitespace-pre-wrap">
                    {selectedFeedback.noiDung || "—"}
                  </p>
                </div>
              </div>

              <button
                onClick={() => setShowDetailModal(false)}
                className="w-full bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
              >
                Đóng
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
