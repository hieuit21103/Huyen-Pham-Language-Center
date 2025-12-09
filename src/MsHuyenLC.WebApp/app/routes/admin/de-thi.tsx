import { Search, Trash2, FileText, X, Eye, Sparkles } from "lucide-react";
import { useState, useEffect } from "react";
import { 
  getDeThis, 
  deleteDeThi, 
  generatePracticeTest, 
  getDeThiQuestionsGrouped
} from "~/apis/index";
import type { KyNang, CapDo, DoKho, CauHoi, DeThi, CheDoCauHoi } from "~/types/index";
import { setLightTheme } from "./_layout";

export default function AdminExams() {
  const [searchTerm, setSearchTerm] = useState("");
  const [exams, setExams] = useState<DeThi[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadingQuestions, setLoadingQuestions] = useState(false);
  const [showPracticeModal, setShowPracticeModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [viewingExam, setViewingExam] = useState<DeThi | null>(null);
  const [examQuestions, setExamQuestions] = useState<CauHoi[]>([]);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error">("success");

  const [practiceForm, setPracticeForm] = useState({
    kyNang: 0 as KyNang,
    capDo: 0 as CapDo,
    doKho: 0 as DoKho,
    soCauHoi: 10,
    thoiLuongPhut: 30,
    cheDoCauHoi: 0, // 0 = Don, 1 = Nhom
  });

  const handleCreatePractice = () => {
    setMessage("");
    setShowPracticeModal(true);
  };

  useEffect(() => {
    setLightTheme();
    loadExams();
  }, []);

  const loadExams = async () => {
    setLoading(true);
    const response = await getDeThis();
    if (response.success && response.data) {
      setExams(response.data);
    }
    setLoading(false);
  };

  const handleViewDetail = async (exam: DeThi) => {
    setViewingExam(exam);
    setShowDetailModal(true);
    setExamQuestions([]);
    setLoadingQuestions(true);
    
    const response = await getDeThiQuestionsGrouped(exam.id!);
    setLoadingQuestions(false);
    
    if (response.success && response.data) {
      let questionsData: CauHoi[] = [];
      
      const responseData = response.data;
      
      // Backend trả về: { deThi, groupedQuestions, standaloneQuestions, totalQuestions }
      if (responseData.standaloneQuestions && Array.isArray(responseData.standaloneQuestions)) {
        // Lấy câu hỏi đơn (standalone)
        questionsData = responseData.standaloneQuestions.map((item: any) => item.cauHoi || item);
      }
      
      if (responseData.groupedQuestions && Array.isArray(responseData.groupedQuestions)) {
        // Thêm câu hỏi trong nhóm
        const groupedCauHois = responseData.groupedQuestions.map((item: any) => item.cauHoi || item);
        questionsData = [...questionsData, ...groupedCauHois];
      }
      
      // Lọc bỏ null/undefined và sắp xếp theo thứ tự
      questionsData = questionsData
        .filter(q => q && q.id)
        .sort((a: any, b: any) => (a.thuTu || 0) - (b.thuTu || 0));
      
      setExamQuestions(questionsData);
      
      if (questionsData.length === 0) {
        setMessage("Không tìm thấy câu hỏi trong đề thi này");
      }
    } else {
      setExamQuestions([]);
      setMessage(response.message || "Không thể tải danh sách câu hỏi");
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa đề thi này?")) {
      const response = await deleteDeThi(id);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadExams();
      }
    }
  };

  const handleGeneratePractice = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const response = await generatePracticeTest({
      capDo: practiceForm.capDo,
      doKho: practiceForm.doKho,
      kyNang: practiceForm.kyNang,
      soCauHoi: practiceForm.soCauHoi,
      thoiLuongPhut: practiceForm.thoiLuongPhut,
      cheDoCauHoi: practiceForm.cheDoCauHoi,
    });
    
    setMessage(response.message || "");
    setMessageType(response.success ? "success" : "error");
    if (response.success) {
      loadExams();
      setShowPracticeModal(false);
      setPracticeForm({
        kyNang: 0,
        capDo: 0,
        doKho: 0,
        soCauHoi: 10,
        thoiLuongPhut: 30,
        cheDoCauHoi: 0,
      });
    }
  };

  const getCheDoCauHoiText = (cheDo?: CheDoCauHoi) => {
    return cheDo === 1 ? "Nhóm" : "Đơn";
  };

  const getKyNangText = (kyNang?: KyNang) => {
    const texts = ["Listening", "Reading", "Ngữ pháp", "Từ vựng", "Writing"];
    return texts[kyNang || 0];
  };

  const getCapDoText = (capDo?: CapDo) => {
    const texts = ["A1", "A2", "B1", "B2", "C1", "C2"];
    return texts[capDo || 0];
  };

  const getDoKhoText = (doKho?: DoKho) => {
    const texts = ["Dễ", "Trung bình", "Khó"];
    return texts[doKho || 0];
  };

  const filteredExams = exams.filter(exam =>
    exam.tenDe?.toLowerCase().includes(searchTerm.toLowerCase())
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
          <h1 className="text-3xl font-bold text-gray-900">Quản lý đề thi</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả đề thi</p>
        </div>
        <button 
          onClick={handleCreatePractice}
          className="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center space-x-2"
        >
          <Sparkles className="w-5 h-5" />
          <span>Tạo đề luyện tập</span>
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm đề thi..."
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
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredExams.map((exam) => (
            <div key={exam.id} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              <div className="p-6">
                <div className="flex items-start justify-between mb-4">
                  <div className="w-12 h-12 bg-gray-900 rounded-lg flex items-center justify-center">
                    <FileText className="w-6 h-6 text-white" />
                  </div>
                  <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${
                    exam.kyThiId ? 'bg-purple-100 text-purple-800' : 'bg-green-100 text-green-800'
                  }`}>
                    {exam.kyThiId ? 'Thi thật' : 'Luyện tập'}
                  </span>
                </div>
                
                <h3 className="text-xl font-bold text-gray-900 mb-4">{exam.tenDe}</h3>
                
                <div className="space-y-2 mb-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Số câu hỏi:</span>
                    <span className="font-semibold text-gray-900">{exam.tongCauHoi || 0}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Thời gian:</span>
                    <span className="font-semibold text-gray-900">{exam.thoiLuongPhut || 0} phút</span>
                  </div>
                </div>
                
                <div className="flex space-x-2 pt-4 border-t border-gray-200">
                  <button 
                    onClick={() => handleViewDetail(exam)}
                    className="flex-1 bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center justify-center space-x-2"
                  >
                    <Eye className="w-4 h-4" />
                    <span>Xem chi tiết</span>
                  </button>
                  <button 
                    onClick={() => handleDelete(exam.id!)}
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
      )}

      {/* View Detail Modal */}
      {showDetailModal && viewingExam && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">Chi tiết đề thi</h2>
                <button onClick={() => setShowDetailModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <div className="space-y-6">
                {/* Exam Info */}
                <div className="bg-gray-50 p-4 rounded-lg space-y-3">
                  <div className="flex justify-between">
                    <span className="font-medium text-gray-700">Tên đề thi:</span>
                    <span className="text-gray-900">{viewingExam.tenDe}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="font-medium text-gray-700">Loại:</span>
                    <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${
                      viewingExam.kyThiId ? 'bg-purple-100 text-purple-800' : 'bg-green-100 text-green-800'
                    }`}>
                      {viewingExam.kyThiId ? 'Thi thật' : 'Luyện tập'}
                    </span>
                  </div>
                  {viewingExam.kyThiId && (
                    <div className="flex justify-between">
                      <span className="font-medium text-gray-700">Kỳ thi:</span>
                      <span className="text-gray-900">{viewingExam.kyThi?.tenKyThi || "N/A"}</span>
                    </div>
                  )}
                  <div className="flex justify-between">
                    <span className="font-medium text-gray-700">Số câu hỏi:</span>
                    <span className="text-gray-900">{viewingExam.tongCauHoi || 0}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="font-medium text-gray-700">Thời gian làm bài:</span>
                    <span className="text-gray-900">{viewingExam.thoiLuongPhut || 0} phút</span>
                  </div>
                </div>

                {/* Questions List */}
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-4">
                    Danh sách câu hỏi ({examQuestions.length})
                  </h3>
                  {loadingQuestions ? (
                    <div className="text-center py-12 bg-gray-50 rounded-lg">
                      <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
                      <p className="text-gray-500 mt-4">Đang tải câu hỏi...</p>
                    </div>
                  ) : examQuestions.length === 0 ? (
                    <div className="text-center py-12 bg-gray-50 rounded-lg">
                      <p className="text-gray-500">Không có câu hỏi nào trong đề thi này</p>
                    </div>
                  ) : (
                    <div className="space-y-4">
                      {examQuestions.map((question, index) => {
                        const questionData = question as any;
                        return (
                          <div key={question.id} className="border border-gray-200 rounded-lg p-4">
                            <div className="flex items-start space-x-3">
                              <span className="flex-shrink-0 w-8 h-8 bg-gray-900 text-white rounded-full flex items-center justify-center font-semibold text-sm">
                                {index + 1}
                              </span>
                              <div className="flex-1">
                                <p className="text-gray-900 mb-3 font-medium">
                                  {questionData.noiDungCauHoi || questionData.noiDung || 'Không có nội dung'}
                                </p>
                                
                                <div className="flex gap-2 flex-wrap mb-3">
                                  <span className="text-xs px-2 py-1 bg-blue-200 text-blue-700 rounded">
                                    {getKyNangText(question.kyNang)}
                                  </span>
                                  <span className="text-xs px-2 py-1 bg-green-200 text-green-700 rounded">
                                    {getCapDoText(question.capDo)}
                                  </span>
                                  <span className="text-xs px-2 py-1 bg-purple-200 text-purple-700 rounded">
                                    {getDoKhoText(question.doKho)}
                                  </span>
                                </div>

                                {/* Hình ảnh */}
                                {(questionData.urlHinhAnh || questionData.urlHinh) && (
                                  <img 
                                    src={questionData.urlHinhAnh || questionData.urlHinh} 
                                    alt="Question" 
                                    className="mt-3 max-w-md rounded-lg shadow-sm" 
                                  />
                                )}

                                {/* Âm thanh */}
                                {questionData.urlAmThanh && (
                                  <div className="mt-3">
                                    <audio controls className="w-full max-w-md">
                                      <source src={questionData.urlAmThanh} type="audio/mpeg" />
                                    </audio>
                                  </div>
                                )}

                                {/* Các đáp án */}
                                {(questionData.cacDapAn || questionData.dapAns) && (questionData.cacDapAn || questionData.dapAns).length > 0 && (
                                  <div className="mt-3 space-y-2">
                                    <p className="text-sm font-medium text-gray-700">Các đáp án:</p>
                                    {(questionData.cacDapAn || questionData.dapAns).map((dapAn: any, idx: number) => (
                                      <div 
                                        key={idx}
                                        className={`p-3 rounded-lg border ${
                                          dapAn.dung 
                                            ? 'bg-green-50 border-green-300' 
                                            : 'bg-gray-50 border-gray-200'
                                        }`}
                                      >
                                        <div className="flex items-start gap-2">
                                          <span className={`font-semibold ${
                                            dapAn.dung ? 'text-green-700' : 'text-gray-700'
                                          }`}>
                                            {dapAn.nhan || String.fromCharCode(65 + idx)}.
                                          </span>
                                          <span className={`flex-1 ${
                                            dapAn.dung ? 'text-green-900 font-medium' : 'text-gray-700'
                                          }`}>
                                            {dapAn.noiDung}
                                          </span>
                                          {dapAn.dung && (
                                            <span className="text-xs bg-green-600 text-white px-2 py-1 rounded">
                                              Đúng
                                            </span>
                                          )}
                                        </div>
                                        {dapAn.giaiThich && (
                                          <p className="mt-2 text-xs text-gray-600 italic pl-6">
                                            {dapAn.giaiThich}
                                          </p>
                                        )}
                                      </div>
                                    ))}
                                  </div>
                                )}

                                {/* Đáp án đúng (nếu không có dapAns) */}
                                {!questionData.cacDapAn && !questionData.dapAns && questionData.dapAnDung && (
                                  <div className="mt-3 p-3 bg-green-50 border border-green-200 rounded-lg">
                                    <span className="font-medium text-green-800">Đáp án: </span>
                                    <span className="text-green-900">{questionData.dapAnDung}</span>
                                  </div>
                                )}

                                {/* Giải thích */}
                                {questionData.giaiThich && (
                                  <div className="mt-2 p-3 bg-blue-50 border border-blue-200 rounded-lg">
                                    <span className="font-medium text-blue-800">Giải thích: </span>
                                    <span className="text-blue-900">{questionData.giaiThich}</span>
                                  </div>
                                )}
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  )}
                </div>

                <div className="flex justify-end pt-4">
                  <button
                    onClick={() => setShowDetailModal(false)}
                    className="bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    Đóng
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Modal tạo đề luyện tập */}
      {showPracticeModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex items-center justify-between mb-6">
                <div>
                  <h2 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
                    <Sparkles className="w-6 h-6 text-purple-600" />
                    Tạo đề luyện tập tự động
                  </h2>
                  <p className="text-gray-600 mt-1">Hệ thống sẽ tự động chọn câu hỏi phù hợp</p>
                </div>
                <button 
                  onClick={() => setShowPracticeModal(false)}
                  className="text-gray-500 hover:text-gray-700"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleGeneratePractice} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Kỹ năng</label>
                    <select
                      value={practiceForm.kyNang}
                      onChange={(e) => setPracticeForm({ ...practiceForm, kyNang: Number(e.target.value) as KyNang })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-purple-600"
                      required
                    >
                      <option value="0">Listening (Nghe)</option>
                      <option value="1">Reading (Đọc)</option>
                      <option value="2">Ngữ pháp</option>
                      <option value="3">Từ vựng</option>
                      <option value="4">Writing (Viết)</option>
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Cấp độ</label>
                    <select
                      value={practiceForm.capDo}
                      onChange={(e) => setPracticeForm({ ...practiceForm, capDo: Number(e.target.value) as CapDo })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-purple-600"
                      required
                    >
                      <option value="0">A1</option>
                      <option value="1">A2</option>
                      <option value="2">B1</option>
                      <option value="3">B2</option>
                      <option value="4">C1</option>
                      <option value="5">C2</option>
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Độ khó</label>
                    <select
                      value={practiceForm.doKho}
                      onChange={(e) => setPracticeForm({ ...practiceForm, doKho: Number(e.target.value) as DoKho })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-purple-600"
                      required
                    >
                      <option value="0">Dễ</option>
                      <option value="1">Trung bình</option>
                      <option value="2">Khó</option>
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Chế độ câu hỏi</label>
                    <select
                      value={practiceForm.cheDoCauHoi}
                      onChange={(e) => setPracticeForm({ ...practiceForm, cheDoCauHoi: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-purple-600"
                      required
                    >
                      <option value="0">Câu hỏi đơn</option>
                      <option value="1">Nhóm câu hỏi</option>
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      {practiceForm.cheDoCauHoi === 0 ? 'Số câu hỏi' : 'Số nhóm câu hỏi'}
                    </label>
                    <input
                      type="number"
                      min="1"
                      max="100"
                      value={practiceForm.soCauHoi}
                      onChange={(e) => setPracticeForm({ ...practiceForm, soCauHoi: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-purple-600"
                      required
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Thời lượng (phút)</label>
                    <input
                      type="number"
                      min="5"
                      max="180"
                      value={practiceForm.thoiLuongPhut}
                      onChange={(e) => setPracticeForm({ ...practiceForm, thoiLuongPhut: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-purple-600"
                      required
                    />
                  </div>
                </div>

                <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
                  <p className="text-sm text-purple-800">
                    💡 <strong>Lưu ý:</strong> Hệ thống sẽ tự động chọn ngẫu nhiên câu hỏi/nhóm câu hỏi phù hợp với tiêu chí bạn chọn.
                    Mỗi lần tạo sẽ có đề khác nhau.
                  </p>
                </div>

                {message && messageType === "error" && (
                  <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-lg flex items-start space-x-2">
                    <X className="w-5 h-5 flex-shrink-0 mt-0.5" />
                    <span>{message}</span>
                  </div>
                )}

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-purple-600 text-white px-6 py-3 rounded-lg hover:bg-purple-700 transition-colors flex items-center justify-center gap-2"
                  >
                    <Sparkles className="w-5 h-5" />
                    Tạo đề luyện tập
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowPracticeModal(false)}
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
