import { Search, Plus, Edit, Trash2, FileQuestion, X, Filter, Upload, Image, Music, Loader2, Copy, Download, BarChart3, Trash, FileSpreadsheet, List } from "lucide-react";
import { useState, useEffect, useRef } from "react";
import { 
  getCauHois, 
  createCauHoi, 
  updateCauHoi, 
  deleteCauHoi, 
  deleteBulkCauHois,
  importCauHoisFromExcel,
  downloadImportTemplate
} from "~/apis/CauHoi";
import { getNhomCauHois } from "~/apis/NhomCauHoi";
import { uploadImage, uploadAudio } from "~/apis/Upload";
import Pagination from "~/components/Pagination";
import type { KyNang, CapDo, DoKho, NhomCauHoiChiTiet, CheDoCauHoi } from "~/types/index";
import { setLightTheme } from "./_layout";
import type { NhomCauHoi, CauHoi } from "~/types/index";
import type { DapAnCauHoi } from "~/types/exam.types";

export default function AdminQuestionBank() {
  const [searchTerm, setSearchTerm] = useState("");
  const [questions, setQuestions] = useState<CauHoi[]>([]);
  const [nhomCauHois, setNhomCauHois] = useState<NhomCauHoi[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [showStatsModal, setShowStatsModal] = useState(false);
  const [showImportModal, setShowImportModal] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<CauHoi | null>(null);
  const [message, setMessage] = useState("");
  const [statistics, setStatistics] = useState<any>(null);
  const [selectedQuestions, setSelectedQuestions] = useState<string[]>([]);
  const [importFile, setImportFile] = useState<File | null>(null);
  
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  
  const [filterKyNang, setFilterKyNang] = useState<string>("");
  const [filterCapDo, setFilterCapDo] = useState<string>("");
  const [filterDoKho, setFilterDoKho] = useState<string>("");
  
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [audioFile, setAudioFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string>("");
  const [audioPreview, setAudioPreview] = useState<string>("");
  const [uploadingImage, setUploadingImage] = useState(false);
  const [uploadingAudio, setUploadingAudio] = useState(false);
  const [dragActiveImage, setDragActiveImage] = useState(false);
  const [dragActiveAudio, setDragActiveAudio] = useState(false);
  
  const imageInputRef = useRef<HTMLInputElement>(null);
  const audioInputRef = useRef<HTMLInputElement>(null);
  
  const [formData, setFormData] = useState({
    noiDungCauHoi: "",
    kyNang: 0 as KyNang,
    urlHinhAnh: "",
    urlAmThanh: "",
    loiThoai: "",
    capDo: 0 as CapDo,
    doKho: 0 as DoKho,
  });

  const [dapAns, setDapAns] = useState<DapAnCauHoi[]>([
    { nhan: "A", noiDung: "", dung: false, giaiThich: "" },
    { nhan: "B", noiDung: "", dung: false, giaiThich: "" },
    { nhan: "C", noiDung: "", dung: false, giaiThich: "" },
    { nhan: "D", noiDung: "", dung: false, giaiThich: "" },
  ]);

  useEffect(() => {
    setLightTheme();
  }, []);

  useEffect(() => {
    setCurrentPage(1); 
  }, [filterKyNang, filterCapDo, filterDoKho, searchTerm]);

  useEffect(() => {
    loadQuestions();
    loadNhomCauHois();
  }, []);

  const loadQuestions = async () => {
    setLoading(true);
    const response = await getCauHois();
    if (response.success && Array.isArray(response.data)) {
      setQuestions(response.data);
    }
    setLoading(false);
  };

  const loadNhomCauHois = async () => {
    const response = await getNhomCauHois();
    if (response.success && Array.isArray(response.data)) {
      setNhomCauHois(response.data);
    }
  };

  const handleCreate = () => {
    setEditingQuestion(null);
    setFormData({
      noiDungCauHoi: "",
      kyNang: 0,
      urlHinhAnh: "",
      urlAmThanh: "",
      loiThoai: "",
      capDo: 0,
      doKho: 0,
    });
    setDapAns([
      { nhan: "A", noiDung: "", dung: false, giaiThich: "" },
      { nhan: "B", noiDung: "", dung: false, giaiThich: "" },
      { nhan: "C", noiDung: "", dung: false, giaiThich: "" },
      { nhan: "D", noiDung: "", dung: false, giaiThich: "" },
    ]);
    setImageFile(null);
    setAudioFile(null);
    setImagePreview("");
    setAudioPreview("");
    setShowModal(true);
  };

  const handleEdit = (question: CauHoi) => {
    setEditingQuestion(question);
    setFormData({
      noiDungCauHoi: question.noiDungCauHoi || "",
      kyNang: question.kyNang ?? 0,
      urlHinhAnh: question.urlHinh || "",
      urlAmThanh: question.urlAmThanh || "",
      loiThoai: question.loiThoai || "",
      capDo: question.capDo ?? 0,
      doKho: question.doKho ?? 0,
    });
    setDapAns(question.cacDapAn || [
      { nhan: "A", noiDung: "", dung: false, giaiThich: "" },
      { nhan: "B", noiDung: "", dung: false, giaiThich: "" },
      { nhan: "C", noiDung: "", dung: false, giaiThich: "" },
      { nhan: "D", noiDung: "", dung: false, giaiThich: "" },
    ]);
    setImageFile(null);
    setAudioFile(null);
    setImagePreview(question.urlHinh || "");
    setAudioPreview(question.urlAmThanh || "");
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    let imageUrl = formData.urlHinhAnh;
    if (imageFile) {
      setUploadingImage(true);
      const uploadResult = await uploadImage(imageFile);
      setUploadingImage(false);
      if (uploadResult.success && uploadResult.url) {
        imageUrl = uploadResult.url;
      } else {
        setMessage(uploadResult.message);
        return;
      }
    }

    let audioUrl = formData.urlAmThanh;
    if (audioFile) {
      setUploadingAudio(true);
      const uploadResult = await uploadAudio(audioFile);
      setUploadingAudio(false);
      if (uploadResult.success && uploadResult.url) {
        audioUrl = uploadResult.url;
      } else {
        setMessage(uploadResult.message);
        return;
      }
    }

    const submitData = {
      ...formData,
      urlHinhAnh: imageUrl,
      urlAmThanh: audioUrl,
      cacDapAn: dapAns.filter(d => d.noiDung?.trim()),
    };
    
    if (editingQuestion) {
      const response = await updateCauHoi(editingQuestion.id!, submitData);
      setMessage(response.message || "");
      if (response.success) {
        loadQuestions();
        setShowModal(false);
        setTimeout(() => setMessage(""), 3000);
      }
    } else {
      const response = await createCauHoi(submitData);
      setMessage(response.message || "");
      if (response.success) {
        loadQuestions();
        setShowModal(false);
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa câu hỏi này?")) {
      const response = await deleteCauHoi(id);
      setMessage(response.message || "");
      if (response.success) {
        loadQuestions();
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const handleBulkDelete = async () => {
    if (selectedQuestions.length === 0) {
      setMessage("Vui lòng chọn ít nhất một câu hỏi");
      setTimeout(() => setMessage(""), 3000);
      return;
    }
    
    if (confirm(`Bạn có chắc chắn muốn xóa ${selectedQuestions.length} câu hỏi đã chọn?`)) {
      const response = await deleteBulkCauHois(selectedQuestions);
      setMessage(response.message || "");
      if (response.success) {
        setSelectedQuestions([]);
        loadQuestions();
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const handleDownloadTemplate = async () => {
    const response = await downloadImportTemplate();
    if (response.success && response.data) {
      const blob = response.data as Blob;
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'Mau_Import_CauHoi.xlsx';
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } else {
      setMessage(response.message || "Tải template thất bại");
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const handleImportExcel = async () => {
    if (!importFile) {
      setMessage("Vui lòng chọn file Excel");
      setTimeout(() => setMessage(""), 3000);
      return;
    }

    setLoading(true);
    const response = await importCauHoisFromExcel(importFile);
    setLoading(false);
    setMessage(response.message || "");
    
    if (response.success) {
      setShowImportModal(false);
      setImportFile(null);
      loadQuestions();
    }
    setTimeout(() => setMessage(""), 3000);
  };

  const toggleSelectQuestion = (id: string) => {
    setSelectedQuestions(prev =>
      prev.includes(id) ? prev.filter(qId => qId !== id) : [...prev, id]
    );
  };

  const toggleSelectAll = () => {
    if (selectedQuestions.length === getPaginatedData().length) {
      setSelectedQuestions([]);
    } else {
      setSelectedQuestions(getPaginatedData().map(q => q.id!));
    }
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file && file.type.startsWith('image/')) {
      setImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleImageDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragActiveImage(false);
    const file = e.dataTransfer.files?.[0];
    if (file && file.type.startsWith('image/')) {
      setImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleImageDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setDragActiveImage(true);
  };

  const handleImageDragLeave = () => {
    setDragActiveImage(false);
  };

  const removeImage = () => {
    setImageFile(null);
    setImagePreview("");
    setFormData({ ...formData, urlHinhAnh: "" });
    if (imageInputRef.current) {
      imageInputRef.current.value = "";
    }
  };

  const handleAudioChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file && file.type.startsWith('audio/')) {
      setAudioFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setAudioPreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleAudioDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragActiveAudio(false);
    const file = e.dataTransfer.files?.[0];
    if (file && file.type.startsWith('audio/')) {
      setAudioFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setAudioPreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleAudioDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setDragActiveAudio(true);
  };

  const handleAudioDragLeave = () => {
    setDragActiveAudio(false);
  };

  const removeAudio = () => {
    setAudioFile(null);
    setAudioPreview("");
    setFormData({ ...formData, urlAmThanh: "" });
    if (audioInputRef.current) {
      audioInputRef.current.value = "";
    }
  };

  const getKyNangText = (kyNang?: KyNang) => {
    switch(kyNang) {
      case 0: return "Listening";
      case 1: return "Reading";
      case 2: return "Grammar";
      case 3: return "Vocabulary";
      case 4: return "Writing";
      default: return "—";
    }
  };

  const getCapDoText = (capDo?: CapDo) => {
    const levels = ["A1", "A2", "B1", "B2", "C1", "C2"];
    return levels[capDo ?? 0] || "—";
  };

  const getDoKhoText = (doKho?: DoKho) => {
    switch(doKho) {
      case 0: return "Dễ";
      case 1: return "Trung bình";
      case 2: return "Khó";
      default: return "—";
    }
  };

  const getDoKhoColor = (doKho?: DoKho) => {
    switch(doKho) {
      case 0: return "bg-green-100 text-green-800";
      case 1: return "bg-yellow-100 text-yellow-800";
      case 2: return "bg-red-100 text-red-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const filteredQuestions = questions.filter((q) => {
    // Filter by search term
    if (searchTerm && !q.noiDungCauHoi?.toLowerCase().includes(searchTerm.toLowerCase())) {
      return false;
    }
    
    // Filter by kyNang
    if (filterKyNang && q.kyNang !== parseInt(filterKyNang)) {
      return false;
    }
    
    // Filter by capDo
    if (filterCapDo && q.capDo !== parseInt(filterCapDo)) {
      return false;
    }
    
    // Filter by doKho
    if (filterDoKho && q.doKho !== parseInt(filterDoKho)) {
      return false;
    }
    
    return true;
  });

  const getPaginatedData = () => {
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    return filteredQuestions.slice(startIndex, endIndex);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const stats = {
    total: filteredQuestions.length,
    tracNghiem: filteredQuestions.filter(q => q.kyNang !== 4).length,
    tuLuan: filteredQuestions.filter(q => q.kyNang === 4).length,
    listening: filteredQuestions.filter(q => q.kyNang === 0).length,
    reading: filteredQuestions.filter(q => q.kyNang === 1).length,
    grammar: filteredQuestions.filter(q => q.kyNang === 2).length,
    vocabulary: filteredQuestions.filter(q => q.kyNang === 3).length,
    writing: filteredQuestions.filter(q => q.kyNang === 4).length,
  };

  return (
    <div className="space-y-6">
      {message && (
        <div className={`${message.includes("thất bại") || message.includes("Lỗi") ? "bg-red-100 border-red-400 text-red-700" : "bg-green-100 border-green-400 text-green-700"} border px-4 py-3 rounded-lg`}>
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Ngân hàng câu hỏi</h1>
          <p className="text-gray-600 mt-1">Quản lý câu hỏi thi</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <a 
            href="/admin/nhom-cau-hoi"
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors flex items-center space-x-2"
          >
            <List className="w-5 h-5" />
            <span>Nhóm câu hỏi</span>
          </a>
          <button 
            onClick={() => setShowImportModal(true)}
            className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2"
          >
            <FileSpreadsheet className="w-5 h-5" />
            <span>Import Excel</span>
          </button>
          <button 
            onClick={handleDownloadTemplate}
            className="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center space-x-2"
          >
            <Download className="w-5 h-5" />
            <span>Tải Template</span>
          </button>
          {selectedQuestions.length > 0 && (
            <button 
              onClick={handleBulkDelete}
              className="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors flex items-center space-x-2"
            >
              <Trash className="w-5 h-5" />
              <span>Xóa ({selectedQuestions.length})</span>
            </button>
          )}
          <button 
            onClick={handleCreate}
            className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
          >
            <Plus className="w-5 h-5" />
            <span>Thêm câu hỏi</span>
          </button>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <p className="text-blue-700 text-xs font-medium mb-1">Tổng số</p>
          <p className="text-2xl font-bold text-blue-900">{stats.total}</p>
        </div>
        <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
          <p className="text-purple-700 text-xs font-medium mb-1">Trắc nghiệm</p>
          <p className="text-2xl font-bold text-purple-900">{stats.tracNghiem}</p>
        </div>
        <div className="bg-pink-50 border border-pink-200 rounded-lg p-4">
          <p className="text-pink-700 text-xs font-medium mb-1">Tự luận</p>
          <p className="text-2xl font-bold text-pink-900">{stats.tuLuan}</p>
        </div>
        <div className="bg-green-50 border border-green-200 rounded-lg p-4">
          <p className="text-green-700 text-xs font-medium mb-1">Reading</p>
          <p className="text-2xl font-bold text-green-900">{stats.reading}</p>
        </div>
        <div className="bg-orange-50 border border-orange-200 rounded-lg p-4">
          <p className="text-orange-700 text-xs font-medium mb-1">Writing</p>
          <p className="text-2xl font-bold text-orange-900">{stats.writing}</p>
        </div>
        <div className="bg-cyan-50 border border-cyan-200 rounded-lg p-4">
          <p className="text-cyan-700 text-xs font-medium mb-1">Listening</p>
          <p className="text-2xl font-bold text-cyan-900">{stats.listening}</p>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Tìm kiếm câu hỏi..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>
          <select
            value={filterKyNang}
            onChange={(e) => setFilterKyNang(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
            <option value="">Tất cả kỹ năng</option>
            <option value="0">Listening</option>
            <option value="1">Reading</option>
            <option value="2">Grammar</option>
            <option value="3">Vocabulary</option>
            <option value="4">Writing</option>
          </select>
          <select
            value={filterCapDo}
            onChange={(e) => setFilterCapDo(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
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
            onChange={(e) => setFilterDoKho(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
            <option value="">Tất cả độ khó</option>
            <option value="0">Dễ</option>
            <option value="1">Trung bình</option>
            <option value="2">Khó</option>
          </select>
        </div>
      </div>

      {/* Questions Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : filteredQuestions.length === 0 ? (
          <div className="p-8 text-center">
            <FileQuestion className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Không tìm thấy câu hỏi nào</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-4 py-3 text-left">
                    <input
                      type="checkbox"
                      checked={selectedQuestions.length === getPaginatedData().length && getPaginatedData().length > 0}
                      onChange={toggleSelectAll}
                      className="rounded border-gray-300 text-gray-900 focus:ring-gray-900"
                    />
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Nội dung
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Loại
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Kỹ năng
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Cấp độ
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Độ khó
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {getPaginatedData().map((question) => (
                  <tr key={question.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-4">
                      <input
                        type="checkbox"
                        checked={selectedQuestions.includes(question.id!)}
                        onChange={() => toggleSelectQuestion(question.id!)}
                        className="rounded border-gray-300 text-gray-900 focus:ring-gray-900"
                      />
                    </td>
                    <td className="px-6 py-4">
                      <p className="text-sm text-gray-900 line-clamp-2">{question.noiDungCauHoi || "—"}</p>
                      {question.cacDapAn && question.cacDapAn.length > 0 && (
                        <p className="text-xs text-gray-500 mt-1">
                          {question.cacDapAn.filter(d => d.dung).map(d => d.nhan).join(", ")} đúng
                        </p>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 text-xs font-semibold rounded-full ${
                        question.kyNang === 4 ? 'bg-pink-100 text-pink-700' : 'bg-blue-100 text-blue-700'
                      }`}>
                        {question.kyNang === 4 ? 'Tự luận' : 'Trắc nghiệm'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {getKyNangText(question.kyNang)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                      {getCapDoText(question.capDo)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getDoKhoColor(question.doKho)}`}>
                        {getDoKhoText(question.doKho)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end space-x-2">
                        <button
                          onClick={() => handleEdit(question)}
                          className="text-blue-600 hover:text-blue-900 p-2 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Chỉnh sửa"
                        >
                          <Edit className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDelete(question.id!)}
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

      {/* Pagination */}
      {!loading && filteredQuestions.length > pageSize && (
        <div className="mt-6">
          <Pagination
            currentPage={currentPage}
            totalCount={filteredQuestions.length}
            pageSize={pageSize}
            onPageChange={handlePageChange}
          />
        </div>
      )}

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-3xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingQuestion ? "Chỉnh sửa câu hỏi" : "Thêm câu hỏi mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Nội dung câu hỏi *</label>
                  <textarea
                    value={formData.noiDungCauHoi}
                    onChange={(e) => setFormData({ ...formData, noiDungCauHoi: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    rows={4}
                    required
                    placeholder="Nhập nội dung câu hỏi..."
                  />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Kỹ năng</label>
                    <select
                      value={formData.kyNang}
                      onChange={(e) => setFormData({ ...formData, kyNang: Number(e.target.value) as KyNang })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value={0}>Listening</option>
                      <option value={1}>Reading</option>
                      <option value={2}>Ngữ pháp</option>
                      <option value={3}>Từ vựng</option>
                      <option value={4}>Writing (Tự luận)</option>
                    </select>
                  </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Cấp độ</label>
                    <select
                      value={formData.capDo}
                      onChange={(e) => setFormData({ ...formData, capDo: Number(e.target.value) as CapDo })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
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
                    <label className="block text-sm font-medium text-gray-700 mb-2">Độ khó</label>
                    <select
                      value={formData.doKho}
                      onChange={(e) => setFormData({ ...formData, doKho: Number(e.target.value) as DoKho })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value={0}>Dễ</option>
                      <option value={1}>Trung bình</option>
                      <option value={2}>Khó</option>
                    </select>
                  </div>
                </div>

                {/* Đáp án */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Đáp án (Cho trắc nghiệm)</label>
                  <div className="space-y-2">
                    {dapAns.map((dapAn, index) => (
                      <div key={index} className="flex items-start space-x-2 bg-gray-50 p-3 rounded-lg">
                        <input
                          type="checkbox"
                          checked={dapAn.dung}
                          onChange={(e) => {
                            const newDapAns = [...dapAns];
                            newDapAns[index].dung = e.target.checked;
                            setDapAns(newDapAns);
                          }}
                          className="mt-1 rounded border-gray-300"
                        />
                        <div className="flex-1 grid grid-cols-12 gap-2">
                          <div className="col-span-1">
                            <input
                              type="text"
                              value={dapAn.nhan}
                              onChange={(e) => {
                                const newDapAns = [...dapAns];
                                newDapAns[index].nhan = e.target.value;
                                setDapAns(newDapAns);
                              }}
                              placeholder="A"
                              className="w-full px-2 py-1 border border-gray-300 rounded text-center font-bold"
                            />
                          </div>
                          <div className="col-span-11">
                            <input
                              type="text"
                              value={dapAn.noiDung}
                              onChange={(e) => {
                                const newDapAns = [...dapAns];
                                newDapAns[index].noiDung = e.target.value;
                                setDapAns(newDapAns);
                              }}
                              placeholder="Nội dung đáp án..."
                              className="w-full px-3 py-1 border border-gray-300 rounded"
                            />
                          </div>
                        </div>
                      </div>
                    ))}
                    <button
                      type="button"
                      onClick={() => setDapAns([...dapAns, { nhan: String.fromCharCode(65 + dapAns.length), noiDung: "", dung: false, giaiThich: "" }])}
                      className="text-sm text-blue-600 hover:text-blue-800 font-medium"
                    >
                      + Thêm đáp án
                    </button>
                  </div>
                </div>

                {/* Lời thoại */}
                <div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Lời thoại (cho Listening)</label>
                    <textarea
                      value={formData.loiThoai}
                      onChange={(e) => setFormData({ ...formData, loiThoai: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      rows={3}
                      placeholder="Nhập transcript lời thoại..."
                    />
                  </div>
                </div>

                {/* Image Upload */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Hình ảnh</label>
                  <div
                    onDrop={handleImageDrop}
                    onDragOver={handleImageDragOver}
                    onDragLeave={handleImageDragLeave}
                    className={`border-2 border-dashed rounded-lg p-6 text-center cursor-pointer transition-colors ${
                      dragActiveImage ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-gray-400'
                    }`}
                    onClick={() => imageInputRef.current?.click()}
                  >
                    <input
                      ref={imageInputRef}
                      type="file"
                      accept="image/*"
                      onChange={handleImageChange}
                      className="hidden"
                    />
                    
                    {uploadingImage ? (
                      <div className="flex flex-col items-center">
                        <Loader2 className="w-12 h-12 text-blue-500 animate-spin mb-2" />
                        <p className="text-sm text-gray-600">Đang tải lên...</p>
                      </div>
                    ) : imagePreview ? (
                      <div className="relative">
                        <img src={imagePreview} alt="Preview" className="max-h-48 mx-auto rounded-lg" />
                        <button
                          type="button"
                          onClick={(e) => {
                            e.stopPropagation();
                            removeImage();
                          }}
                          className="absolute top-2 right-2 bg-red-500 text-white p-2 rounded-full hover:bg-red-600"
                        >
                          <X className="w-4 h-4" />
                        </button>
                        <p className="text-xs text-gray-500 mt-2">
                          {imageFile ? imageFile.name : 'Hình ảnh hiện tại'}
                        </p>
                      </div>
                    ) : (
                      <div className="flex flex-col items-center">
                        <Image className="w-12 h-12 text-gray-400 mb-2" />
                        <p className="text-sm text-gray-600 mb-1">Kéo thả hình ảnh vào đây</p>
                        <p className="text-xs text-gray-500">hoặc click để chọn file</p>
                      </div>
                    )}
                  </div>
                </div>

                {/* Audio Upload */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Âm thanh</label>
                  <div
                    onDrop={handleAudioDrop}
                    onDragOver={handleAudioDragOver}
                    onDragLeave={handleAudioDragLeave}
                    className={`border-2 border-dashed rounded-lg p-6 text-center cursor-pointer transition-colors ${
                      dragActiveAudio ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-gray-400'
                    }`}
                    onClick={() => audioInputRef.current?.click()}
                  >
                    <input
                      ref={audioInputRef}
                      type="file"
                      accept="audio/*"
                      onChange={handleAudioChange}
                      className="hidden"
                    />
                    
                    {uploadingAudio ? (
                      <div className="flex flex-col items-center">
                        <Loader2 className="w-12 h-12 text-blue-500 animate-spin mb-2" />
                        <p className="text-sm text-gray-600">Đang tải lên...</p>
                      </div>
                    ) : audioPreview ? (
                      <div className="relative">
                        <audio controls className="w-full mb-2">
                          <source src={audioPreview} />
                        </audio>
                        <button
                          type="button"
                          onClick={(e) => {
                            e.stopPropagation();
                            removeAudio();
                          }}
                          className="absolute top-2 right-2 bg-red-500 text-white p-2 rounded-full hover:bg-red-600"
                        >
                          <X className="w-4 h-4" />
                        </button>
                        <p className="text-xs text-gray-500 mt-2">
                          {audioFile ? audioFile.name : 'Âm thanh hiện tại'}
                        </p>
                      </div>
                    ) : (
                      <div className="flex flex-col items-center">
                        <Music className="w-12 h-12 text-gray-400 mb-2" />
                        <p className="text-sm text-gray-600 mb-1">Kéo thả file âm thanh vào đây</p>
                        <p className="text-xs text-gray-500">hoặc click để chọn file</p>
                      </div>
                    )}
                  </div>
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    disabled={uploadingImage || uploadingAudio}
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center"
                  >
                    {uploadingImage || uploadingAudio ? (
                      <>
                        <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                        Đang tải lên...
                      </>
                    ) : (
                      editingQuestion ? "Cập nhật" : "Thêm mới"
                    )}
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

      {/* Statistics Modal */}
      {showStatsModal && statistics && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">Thống kê câu hỏi</h2>
                <button onClick={() => setShowStatsModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>
              
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                    <p className="text-blue-700 text-sm font-medium mb-1">Tổng số câu hỏi</p>
                    <p className="text-3xl font-bold text-blue-900">{statistics.total || 0}</p>
                  </div>
                  <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
                    <p className="text-purple-700 text-sm font-medium mb-1">Trắc nghiệm</p>
                    <p className="text-3xl font-bold text-purple-900">{statistics.tracNghiem || 0}</p>
                  </div>
                  <div className="bg-pink-50 border border-pink-200 rounded-lg p-4">
                    <p className="text-pink-700 text-sm font-medium mb-1">Tự luận</p>
                    <p className="text-3xl font-bold text-pink-900">{statistics.tuLuan || 0}</p>
                  </div>
                  <div className="bg-green-50 border border-green-200 rounded-lg p-4">
                    <p className="text-green-700 text-sm font-medium mb-1">Reading</p>
                    <p className="text-3xl font-bold text-green-900">{statistics.reading || 0}</p>
                  </div>
                  <div className="bg-orange-50 border border-orange-200 rounded-lg p-4">
                    <p className="text-orange-700 text-sm font-medium mb-1">Writing</p>
                    <p className="text-3xl font-bold text-orange-900">{statistics.writing || 0}</p>
                  </div>
                  <div className="bg-cyan-50 border border-cyan-200 rounded-lg p-4">
                    <p className="text-cyan-700 text-sm font-medium mb-1">Listening</p>
                    <p className="text-3xl font-bold text-cyan-900">{statistics.listening || 0}</p>
                  </div>
                </div>
                
                {statistics.byLevel && (
                  <div className="border-t pt-4">
                    <h3 className="font-semibold text-gray-900 mb-3">Theo cấp độ</h3>
                    <div className="grid grid-cols-3 gap-3">
                      {Object.entries(statistics.byLevel).map(([level, count]: [string, any]) => (
                        <div key={level} className="bg-gray-50 border border-gray-200 rounded-lg p-3">
                          <p className="text-gray-600 text-xs font-medium">{level}</p>
                          <p className="text-xl font-bold text-gray-900">{count}</p>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
                
                {statistics.byDifficulty && (
                  <div className="border-t pt-4">
                    <h3 className="font-semibold text-gray-900 mb-3">Theo độ khó</h3>
                    <div className="grid grid-cols-3 gap-3">
                      {Object.entries(statistics.byDifficulty).map(([diff, count]: [string, any]) => (
                        <div key={diff} className="bg-gray-50 border border-gray-200 rounded-lg p-3">
                          <p className="text-gray-600 text-xs font-medium">{diff}</p>
                          <p className="text-xl font-bold text-gray-900">{count}</p>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Import Modal */}
      {showImportModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-lg w-full">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">Import câu hỏi từ Excel</h2>
                <button onClick={() => setShowImportModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>
              
              <div className="space-y-4">
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                  <p className="text-sm text-blue-800 mb-2">
                    Vui lòng tải template Excel và điền thông tin theo đúng định dạng.
                  </p>
                  <button
                    onClick={handleDownloadTemplate}
                    className="text-blue-600 hover:text-blue-800 text-sm font-medium flex items-center space-x-1"
                  >
                    <Download className="w-4 h-4" />
                    <span>Tải template mẫu</span>
                  </button>
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Chọn file Excel
                  </label>
                  <input
                    type="file"
                    accept=".xlsx,.xls"
                    onChange={(e) => setImportFile(e.target.files?.[0] || null)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                  {importFile && (
                    <p className="text-sm text-gray-600 mt-2">
                      File đã chọn: {importFile.name}
                    </p>
                  )}
                </div>
                
                <div className="flex space-x-3 pt-4">
                  <button
                    onClick={handleImportExcel}
                    disabled={!importFile || loading}
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
                  >
                    {loading ? (
                      <>
                        <Loader2 className="w-5 h-5 mr-2 animate-spin inline" />
                        Đang import...
                      </>
                    ) : (
                      "Import"
                    )}
                  </button>
                  <button
                    onClick={() => setShowImportModal(false)}
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
