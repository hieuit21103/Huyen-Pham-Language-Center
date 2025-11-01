import { Search, Plus, Edit, Trash2, FileText, X, Clock, Eye, CheckSquare, Square } from "lucide-react";
import { useState, useEffect } from "react";
import { 
  getCauHois,
  getKyThis,
  getDeThis, 
  createDeThi, 
  updateDeThi, 
  deleteDeThi, 
  generateDeThi, 
  getDeThiQuestions } from "~/apis/index";
import type { LoaiDeThi, LoaiCauHoi, KyNang, CapDo, DoKho, CauHoi, DeThi, KyThi } from "~/types/index";
import { setLightTheme } from "./_layout";

export default function AdminExams() {
  const [searchTerm, setSearchTerm] = useState("");
  const [exams, setExams] = useState<DeThi[]>([]);
  const [examPeriods, setExamPeriods] = useState<KyThi[]>([]);
  const [questions, setQuestions] = useState<CauHoi[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadingQuestions, setLoadingQuestions] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [showGenerateModal, setShowGenerateModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [editingExam, setEditingExam] = useState<DeThi | null>(null);
  const [viewingExam, setViewingExam] = useState<DeThi | null>(null);
  const [examQuestions, setExamQuestions] = useState<CauHoi[]>([]);
  const [selectedQuestions, setSelectedQuestions] = useState<string[]>([]);
  const [message, setMessage] = useState("");
  
  // Modal filters
  const [modalSearchTerm, setModalSearchTerm] = useState("");
  const [filterLoaiCauHoi, setFilterLoaiCauHoi] = useState<number | "">("");
  const [filterKyNang, setFilterKyNang] = useState<number | "">("");
  const [filterCapDo, setFilterCapDo] = useState<number | "">("");
  const [filterDoKho, setFilterDoKho] = useState<number | "">("");
  
  const [formData, setFormData] = useState({
    tenDe: "",
    tongCauHoi: 0,
    thoiGianLamBai: 0,
    kyThiId: "",
    loaiDeThi: 0 as LoaiDeThi,
    cauHoiIds: [] as string[],
  });

  const [generateForm, setGenerateForm] = useState({
    tenDe: "",
    tongCauHoi: 30,
    thoiGianLamBai: 60,
    loaiDeThi: 0 as LoaiDeThi,
    loaiCauHoi: 0 as LoaiCauHoi,
    kyNang: 0 as KyNang,
    capDo: 0 as CapDo,
    doKho: 0 as DoKho,
  });

  useEffect(() => {
    setLightTheme();
    loadExams();
    loadQuestions();
    loadExamPeriods();
  }, []);

  const loadExams = async () => {
    setLoading(true);
    const response = await getDeThis({ pageNumber: 1, pageSize: 100 });
    if (response.success && response.data) {
      setExams(response.data);
    }
    setLoading(false);
  };

  const loadQuestions = async () => {
    const response = await getCauHois({ pageNumber: 1, pageSize: 1000 });
    if (response.success && response.data) {
      setQuestions(response.data);
    }
  };

  const loadExamPeriods = async () => {
    const response = await getKyThis({ pageNumber: 1, pageSize: 100 });
    if (response.success && response.data) {
      setExamPeriods(response.data);
    }
  };

  const handleCreate = () => {
    setEditingExam(null);
    setSelectedQuestions([]);
    setModalSearchTerm("");
    setFilterLoaiCauHoi("");
    setFilterKyNang("");
    setFilterCapDo("");
    setFilterDoKho("");
    setFormData({
      tenDe: "",
      kyThiId: "",
      tongCauHoi: 0,
      thoiGianLamBai: 0,
      loaiDeThi: 0,
      cauHoiIds: [],
    });
    setShowModal(true);
  };

  const handleGenerateExam = async (e: React.FormEvent) => {
    e.preventDefault();
    const response = await generateDeThi(generateForm);
    setMessage(response.message || "");
    if (response.success) {
      loadExams();
      setShowGenerateModal(false);
    }
  };

  const handleEdit = async (exam: DeThi) => {
    setEditingExam(exam);
    setSelectedQuestions([]);
    setModalSearchTerm("");
    setFilterLoaiCauHoi("");
    setFilterKyNang("");
    setFilterCapDo("");
    setFilterDoKho("");
    setFormData({
      tenDe: exam.tenDe || "",
      kyThiId: exam.kyThiId || "",
      tongCauHoi: exam.tongCauHoi || 0,
      thoiGianLamBai: exam.thoiLuongPhut || 0,
      loaiDeThi: exam.loaiDeThi || 0,
      cauHoiIds: [],
    });
    
    // Load existing questions for this exam
    setLoadingQuestions(true);
    const response = await getDeThiQuestions(exam.id!);
    setLoadingQuestions(false);
    
    if (response.success && response.data) {
      let questionsData: CauHoi[] = [];
      
      // Try to get the actual data
      const actualData = (response.data as any).data || response.data;
      
      if (Array.isArray(actualData)) {
        questionsData = actualData;
      } else if (typeof actualData === 'object' && actualData !== null) {
        // Try different ways to extract array from object
        const possibleArrays = [
          actualData.items,
          actualData.questions,
          actualData.cauHois,
          Object.values(actualData)
        ];
        
        for (const possibleArray of possibleArrays) {
          if (Array.isArray(possibleArray) && possibleArray.length > 0) {
            questionsData = possibleArray;
            break;
          }
        }
        
        // If still empty, try to extract from numeric keys
        if (questionsData.length === 0) {
          const numericKeys = Object.keys(actualData)
            .filter(key => !isNaN(Number(key)))
            .sort((a, b) => Number(a) - Number(b));
          
          if (numericKeys.length > 0) {
            questionsData = numericKeys
              .map(key => actualData[key])
              .filter(item => item && typeof item === 'object' && item.id);
          }
        }
      }
      
      // Set selected questions
      const questionIds = questionsData.map(q => q.id!).filter(Boolean);
      setSelectedQuestions(questionIds);
    }
    
    setShowModal(true);
  };

  const handleViewDetail = async (exam: DeThi) => {
    setViewingExam(exam);
    console.log("Viewing exam:", exam);
    setShowDetailModal(true);
    setExamQuestions([]); 
    setLoadingQuestions(true);
    
    const response = await getDeThiQuestions(exam.id!);
    setLoadingQuestions(false);
    
    if (response.success && response.data) {
      let questionsData: CauHoi[] = [];
      
      const actualData = (response.data as any).data || response.data;
      
      if (Array.isArray(actualData)) {
        questionsData = actualData;
      } else if (typeof actualData === 'object' && actualData !== null) {
        const possibleArrays = [
          actualData.items,
          actualData.questions,
          actualData.cauHois,
          Object.values(actualData)
        ];
        
        for (const possibleArray of possibleArrays) {
          if (Array.isArray(possibleArray) && possibleArray.length > 0) {
            questionsData = possibleArray;
            break;
          }
        }
        
        if (questionsData.length === 0) {
          const numericKeys = Object.keys(actualData)
            .filter(key => !isNaN(Number(key)))
            .sort((a, b) => Number(a) - Number(b));
          
          if (numericKeys.length > 0) {
            questionsData = numericKeys
              .map(key => actualData[key])
              .filter(item => item && typeof item === 'object' && item.id);
          }
        }
      }
      
      setExamQuestions(questionsData);
      
      if (questionsData.length === 0) {
        setMessage("Không tìm thấy câu hỏi trong đề thi này");
      }
    } else {
      setExamQuestions([]);
      setMessage(response.message || "Không thể tải danh sách câu hỏi");
    }
  };

  const toggleQuestionSelection = (questionId: string) => {
    setSelectedQuestions(prev => {
      if (prev.includes(questionId)) {
        return prev.filter(id => id !== questionId);
      } else {
        return [...prev, questionId];
      }
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const submitData = {
      ...formData,
      kyThiId: formData.kyThiId,
      cauHoiIds: selectedQuestions,
      tongCauHoi: selectedQuestions.length,
    };

    if (editingExam) {
      console.log("Submitting update with data:", submitData);
      const response = await updateDeThi(editingExam.id!, submitData);
      setMessage(response.message || "");
      if (response.success) {
        loadExams();
        setShowModal(false);
        setSelectedQuestions([]);
      }
    } else {
      if (selectedQuestions.length === 0) {
        setMessage("Vui lòng chọn ít nhất một câu hỏi");
        return;
      }
      const response = await createDeThi(submitData);
      setMessage(response.message || "");
      if (response.success) {
        loadExams();
        setShowModal(false);
        setSelectedQuestions([]);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa đề thi này?")) {
      const response = await deleteDeThi(id);
      setMessage(response.message || "");
      if (response.success) {
        loadExams();
      }
    }
  };

  const getLoaiDeThiText = (loai?: LoaiDeThi) => {
    return loai === 0 ? "Thực hành" : "Thi";
  };

  const getLoaiCauHoiText = (loai?: LoaiCauHoi) => {
    return loai === 0 ? "Trắc nghiệm" : "Tự luận";
  };

  const getKyNangText = (kyNang?: KyNang) => {
    const texts = ["Reading", "Writing", "Listening"];
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

  // Filter questions in modal
  const filteredModalQuestions = questions.filter(question => {
    const questionData = question as any;
    const content = questionData.noiDungCauHoi || questionData.noiDung || '';
    
    // Search term filter
    if (modalSearchTerm && !content.toLowerCase().includes(modalSearchTerm.toLowerCase())) {
      return false;
    }
    
    // Loại câu hỏi filter
    if (filterLoaiCauHoi !== "" && question.loaiCauHoi !== filterLoaiCauHoi) {
      return false;
    }
    
    // Kỹ năng filter
    if (filterKyNang !== "" && question.kyNang !== filterKyNang) {
      return false;
    }
    
    // Cấp độ filter
    if (filterCapDo !== "" && question.capDo !== filterCapDo) {
      return false;
    }
    
    // Độ khó filter
    if (filterDoKho !== "" && question.doKho !== filterDoKho) {
      return false;
    }
    
    return true;
  });

  const clearFilters = () => {
    setModalSearchTerm("");
    setFilterLoaiCauHoi("");
    setFilterKyNang("");
    setFilterCapDo("");
    setFilterDoKho("");
  };

  return (
    <div className="space-y-6">
      {message && (
        <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg">
          {message}
        </div>
      )}

      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý đề thi</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả đề thi</p>
        </div>
        <div className="flex gap-2">
          <button 
            onClick={() => setShowGenerateModal(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
          >
            <Clock className="w-5 h-5" />
            <span>Tạo tự động</span>
          </button>
          <button 
            onClick={handleCreate}
            className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
          >
            <Plus className="w-5 h-5" />
            <span>Tạo thủ công</span>
          </button>
        </div>
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
                  <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800">
                    {getLoaiDeThiText(exam.loaiDeThi)}
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
                    <span>Xem</span>
                  </button>
                  <button 
                    onClick={() => handleEdit(exam)}
                    className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center space-x-2"
                  >
                    <Edit className="w-4 h-4" />
                    <span>Sửa</span>
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

      {/* Manual Create/Edit Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingExam ? "Cập nhật đề thi" : "Tạo đề thi thủ công"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tên đề thi</label>
                  <input
                    type="text"
                    value={formData.tenDe}
                    onChange={(e) => setFormData({ ...formData, tenDe: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Thời gian (phút)</label>
                    <input
                      type="number"
                      value={formData.thoiGianLamBai}
                      onChange={(e) => setFormData({ ...formData, thoiGianLamBai: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Loại đề thi</label>
                    <select
                      value={formData.loaiDeThi}
                      onChange={(e) => setFormData({ ...formData, loaiDeThi: Number(e.target.value) as LoaiDeThi })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="0">Thực hành</option>
                      <option value="1">Thi</option>
                    </select>
                  </div>
                </div>
                
                {formData.loaiDeThi === 1 && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Kỳ thi</label>
                    <select 
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      value={formData.kyThiId}
                      onChange={(e) => setFormData({ ...formData, kyThiId: e.target.value })}
                    >
                      <option value="">-- Chọn kỳ thi --</option>
                      {examPeriods.map(period => (
                        <option key={period.id} value={period.id}>{period.tenKyThi}</option>
                      ))}
                    </select>
                  </div>
                )}

                <div>
                  <div className="flex items-center justify-between mb-3">
                    <label className="block text-sm font-medium text-gray-700">
                      Chọn câu hỏi ({selectedQuestions.length}/{filteredModalQuestions.length} câu)
                      {editingExam && loadingQuestions && (
                        <span className="ml-2 text-xs text-blue-600">Đang tải câu hỏi hiện tại...</span>
                      )}
                    </label>
                    <button
                      type="button"
                      onClick={clearFilters}
                      className="text-xs text-blue-600 hover:text-blue-700 underline"
                    >
                      Xóa bộ lọc
                    </button>
                  </div>

                  {/* Search and Filters */}
                  <div className="mb-4 space-y-3 p-4 bg-gray-50 rounded-lg border border-gray-200">
                    <div className="relative">
                      <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
                      <input
                        type="text"
                        placeholder="Tìm kiếm câu hỏi..."
                        value={modalSearchTerm}
                        onChange={(e) => setModalSearchTerm(e.target.value)}
                        className="w-full pl-10 pr-4 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
                      />
                    </div>

                    <div className="grid grid-cols-2 lg:grid-cols-4 gap-2">
                      <select
                        value={filterLoaiCauHoi}
                        onChange={(e) => setFilterLoaiCauHoi(e.target.value === "" ? "" : Number(e.target.value))}
                        className="text-sm px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
                      >
                        <option value="">Tất cả loại</option>
                        <option value="0">Trắc nghiệm</option>
                        <option value="1">Tự luận</option>
                      </select>

                      <select
                        value={filterKyNang}
                        onChange={(e) => setFilterKyNang(e.target.value === "" ? "" : Number(e.target.value))}
                        className="text-sm px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
                      >
                        <option value="">Tất cả kỹ năng</option>
                        <option value="0">Reading</option>
                        <option value="1">Writing</option>
                        <option value="2">Listening</option>
                      </select>

                      <select
                        value={filterCapDo}
                        onChange={(e) => setFilterCapDo(e.target.value === "" ? "" : Number(e.target.value))}
                        className="text-sm px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
                      >
                        <option value="">Tất cả cấp độ</option>
                        <option value="0">A1</option>
                        <option value="1">A2</option>
                        <option value="2">B1</option>
                        <option value="3">B2</option>
                        <option value="4">C1</option>
                        <option value="5">C2</option>
                      </select>

                      <select
                        value={filterDoKho}
                        onChange={(e) => setFilterDoKho(e.target.value === "" ? "" : Number(e.target.value))}
                        className="text-sm px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
                      >
                        <option value="">Tất cả độ khó</option>
                        <option value="0">Dễ</option>
                        <option value="1">Trung bình</option>
                        <option value="2">Khó</option>
                      </select>
                    </div>
                  </div>

                  <div className="border border-gray-300 rounded-lg max-h-96 overflow-y-auto">
                    {filteredModalQuestions.length === 0 ? (
                      <div className="p-8 text-center text-gray-500">
                        <p>Không tìm thấy câu hỏi phù hợp</p>
                      </div>
                    ) : (
                      filteredModalQuestions.map((question) => {
                      const questionData = question as any;
                      const isSelected = selectedQuestions.includes(question.id!);
                      
                      return (
                        <div
                          key={question.id}
                          onClick={() => toggleQuestionSelection(question.id!)}
                          className={`p-4 border-b border-gray-200 cursor-pointer hover:bg-gray-50 ${
                            isSelected ? 'bg-blue-50 border-blue-200' : ''
                          }`}
                        >
                          <div className="flex items-start space-x-3">
                            <div className="mt-1 flex-shrink-0">
                              {isSelected ? (
                                <CheckSquare className="w-5 h-5 text-blue-600" />
                              ) : (
                                <Square className="w-5 h-5 text-gray-400" />
                              )}
                            </div>
                            
                            <div className="flex-1 min-w-0">
                              <p className="text-sm font-medium text-gray-900 mb-2">
                                {questionData.noiDungCauHoi || questionData.noiDung || 'Không có nội dung'}
                              </p>
                              
                              <div className="flex gap-2 flex-wrap mb-2">
                                <span className="text-xs px-2 py-1 bg-gray-200 text-gray-700 rounded">
                                  {getLoaiCauHoiText(question.loaiCauHoi)}
                                </span>
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

                              {/* Đoạn văn */}
                              {questionData.doanVan && (
                                <div className="mt-2 p-2 bg-blue-50 border-l-2 border-blue-400 rounded text-xs">
                                  <p className="font-medium text-blue-800 mb-1">Đoạn văn:</p>
                                  <p className="text-gray-700 line-clamp-3">{questionData.doanVan}</p>
                                </div>
                              )}

                              {/* Hình ảnh */}
                              {(questionData.urlHinhAnh || questionData.urlHinh) && (
                                <img 
                                  src={questionData.urlHinhAnh || questionData.urlHinh} 
                                  alt="Question" 
                                  className="mt-2 max-w-xs rounded border shadow-sm" 
                                />
                              )}

                              {/* Âm thanh */}
                              {questionData.urlAmThanh && (
                                <div className="mt-2">
                                  <audio controls className="w-full max-w-xs h-8">
                                    <source src={questionData.urlAmThanh} type="audio/mpeg" />
                                  </audio>
                                </div>
                              )}

                              {/* Các đáp án */}
                              {questionData.dapAns && questionData.dapAns.length > 0 && (
                                <div className="mt-2 space-y-1">
                                  {questionData.dapAns.slice(0, 4).map((dapAn: any, idx: number) => (
                                    <div 
                                      key={idx}
                                      className={`text-xs p-2 rounded ${
                                        dapAn.dung 
                                          ? 'bg-green-100 text-green-800 font-medium' 
                                          : 'bg-gray-100 text-gray-700'
                                      }`}
                                    >
                                      <span className="font-semibold">
                                        {dapAn.nhan || String.fromCharCode(65 + idx)}.
                                      </span>{' '}
                                      {dapAn.noiDung}
                                      {dapAn.dung && (
                                        <span className="ml-2 text-xs bg-green-600 text-white px-1.5 py-0.5 rounded">
                                          ✓
                                        </span>
                                      )}
                                    </div>
                                  ))}
                                </div>
                              )}

                              {/* Đáp án đúng (fallback) */}
                              {!questionData.dapAns && questionData.dapAnDung && (
                                <div className="mt-2 text-xs p-2 bg-green-100 text-green-800 rounded font-medium">
                                  <span className="font-bold">Đáp án:</span> {questionData.dapAnDung}
                                </div>
                              )}
                            </div>
                          </div>
                        </div>
                      );
                    }))}
                  </div>
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingExam ? "Cập nhật" : "Tạo đề thi"}
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
                    <span className="font-medium text-gray-700">Loại đề thi:</span>
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800">
                      {getLoaiDeThiText(viewingExam.loaiDeThi)}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="font-medium text-gray-700">Kỳ thi:</span>
                    <span className="text-gray-900">{viewingExam.kyThi?.tenKyThi || "N/A"}</span>
                  </div>
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
                                  <span className="text-xs px-2 py-1 bg-gray-200 text-gray-700 rounded">
                                    {getLoaiCauHoiText(question.loaiCauHoi)}
                                  </span>
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

                                {/* Đoạn văn đọc hiểu */}
                                {questionData.doanVan && (
                                  <div className="mt-3 p-4 bg-blue-50 border-l-4 border-blue-500 rounded">
                                    <p className="text-sm font-medium text-blue-800 mb-2">Đoạn văn:</p>
                                    <p className="text-gray-700 whitespace-pre-wrap">{questionData.doanVan}</p>
                                  </div>
                                )}

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
                                {questionData.dapAns && questionData.dapAns.length > 0 && (
                                  <div className="mt-3 space-y-2">
                                    <p className="text-sm font-medium text-gray-700">Các đáp án:</p>
                                    {questionData.dapAns.map((dapAn: any, idx: number) => (
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
                                {!questionData.dapAns && questionData.dapAnDung && (
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

      {/* Generate Modal */}
      {showGenerateModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">Tạo đề thi tự động</h2>
                <button onClick={() => setShowGenerateModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleGenerateExam} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tên đề thi</label>
                  <input
                    type="text"
                    value={generateForm.tenDe}
                    onChange={(e) => setGenerateForm({ ...generateForm, tenDe: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Tổng câu hỏi</label>
                    <input
                      type="number"
                      value={generateForm.tongCauHoi}
                      onChange={(e) => setGenerateForm({ ...generateForm, tongCauHoi: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Thời gian (phút)</label>
                    <input
                      type="number"
                      value={generateForm.thoiGianLamBai}
                      onChange={(e) => setGenerateForm({ ...generateForm, thoiGianLamBai: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Loại đề thi</label>
                    <select
                      value={generateForm.loaiDeThi}
                      onChange={(e) => setGenerateForm({ ...generateForm, loaiDeThi: Number(e.target.value) as LoaiDeThi })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="0">Thực hành</option>
                      <option value="1">Thi thử</option>
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Loại câu hỏi</label>
                    <select
                      value={generateForm.loaiCauHoi}
                      onChange={(e) => setGenerateForm({ ...generateForm, loaiCauHoi: Number(e.target.value) as LoaiCauHoi })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="0">Trắc nghiệm</option>
                      <option value="1">Tự luận</option>
                    </select>
                  </div>
                </div>

                <div className="grid grid-cols-3 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Kỹ năng</label>
                    <select
                      value={generateForm.kyNang}
                      onChange={(e) => setGenerateForm({ ...generateForm, kyNang: Number(e.target.value) as KyNang })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="0">Reading</option>
                      <option value="1">Writing</option>
                      <option value="2">Listening</option>
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Cấp độ</label>
                    <select
                      value={generateForm.capDo}
                      onChange={(e) => setGenerateForm({ ...generateForm, capDo: Number(e.target.value) as CapDo })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
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
                      value={generateForm.doKho}
                      onChange={(e) => setGenerateForm({ ...generateForm, doKho: Number(e.target.value) as DoKho })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="0">Dễ</option>
                      <option value="1">Trung bình</option>
                      <option value="2">Khó</option>
                    </select>
                  </div>
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    Tạo đề thi
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowGenerateModal(false)}
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
