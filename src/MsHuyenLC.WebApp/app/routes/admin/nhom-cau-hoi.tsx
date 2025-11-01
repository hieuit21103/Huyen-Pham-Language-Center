import { Search, Plus, Edit, Trash2, Copy, FileText, X, Music, Image as ImageIcon, Loader2, List, FileQuestion, ArrowLeft } from "lucide-react";
import { useState, useEffect } from "react";
import { 
  getNhomCauHois, 
  createNhomCauHoi, 
  updateNhomCauHoi, 
  deleteNhomCauHoi,
  getCauHoisInNhom,
  addCauHoiToNhom,
  removeCauHoiFromNhom,
  cloneNhomCauHoi,
  searchNhomCauHois,
  exportNhomCauHoiToPDF
} from "~/apis/NhomCauHoi";
import { getCauHois } from "~/apis/CauHoi";
import { uploadImage, uploadAudio } from "~/apis/Upload";
import type { CapDo, DoKho } from "~/types/index";
import { setLightTheme } from "./_layout";

interface NhomCauHoi {
  id?: string;
  urlAmThanh?: string;
  urlHinhAnh?: string;
  noiDung?: string;
  tieuDe?: string;
  soLuongCauHoi?: number;
  doKho?: DoKho;
  capDo?: CapDo;
}

interface CauHoi {
  id?: string;
  noiDung?: string;
  dapAnDung?: string;
}

export default function AdminNhomCauHoi() {
  const [searchTerm, setSearchTerm] = useState("");
  const [nhomCauHois, setNhomCauHois] = useState<NhomCauHoi[]>([]);
  const [allCauHois, setAllCauHois] = useState<CauHoi[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [showQuestionsModal, setShowQuestionsModal] = useState(false);
  const [editingNhom, setEditingNhom] = useState<NhomCauHoi | null>(null);
  const [selectedNhom, setSelectedNhom] = useState<NhomCauHoi | null>(null);
  const [nhomQuestions, setNhomQuestions] = useState<CauHoi[]>([]);
  const [message, setMessage] = useState("");
  
  const [filterCapDo, setFilterCapDo] = useState<string>("");
  const [filterDoKho, setFilterDoKho] = useState<string>("");
  
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [audioFile, setAudioFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string>("");
  const [audioPreview, setAudioPreview] = useState<string>("");
  const [uploadingImage, setUploadingImage] = useState(false);
  const [uploadingAudio, setUploadingAudio] = useState(false);
  
  const [formData, setFormData] = useState({
    tieuDe: "",
    noiDung: "",
    urlHinhAnh: "",
    urlAmThanh: "",
    soLuongCauHoi: 0,
    capDo: 0 as CapDo,
    doKho: 0 as DoKho,
  });

  useEffect(() => {
    setLightTheme();
    loadNhomCauHois();
    loadAllCauHois();
  }, []);

  useEffect(() => {
    loadNhomCauHois();
  }, [filterCapDo, filterDoKho, searchTerm]);

  const loadNhomCauHois = async () => {
    setLoading(true);
    
    if (filterCapDo || filterDoKho || searchTerm) {
      const filters: any = {
        pageNumber: 1,
        pageSize: 1000
      };
      
      if (filterCapDo) filters.capDo = parseInt(filterCapDo);
      if (filterDoKho) filters.doKho = parseInt(filterDoKho);
      if (searchTerm) filters.keyword = searchTerm;
      
      const response = await searchNhomCauHois(filters);
      if (response.success && Array.isArray(response.data)) {
        setNhomCauHois(response.data);
      }
    } else {
      const response = await getNhomCauHois({ pageNumber: 1, pageSize: 1000, sortBy: 'id', sortOrder: 'desc' });
      if (response.success && Array.isArray(response.data)) {
        setNhomCauHois(response.data);
      }
    }
    setLoading(false);
  };

  const loadAllCauHois = async () => {
    const response = await getCauHois({ pageNumber: 1, pageSize: 1000 });
    if (response.success && Array.isArray(response.data)) {
      setAllCauHois(response.data);
    }
  };

  const handleCreate = () => {
    setEditingNhom(null);
    setFormData({
      tieuDe: "",
      noiDung: "",
      urlHinhAnh: "",
      urlAmThanh: "",
      soLuongCauHoi: 0,
      capDo: 0,
      doKho: 0,
    });
    setImageFile(null);
    setAudioFile(null);
    setImagePreview("");
    setAudioPreview("");
    setShowModal(true);
  };

  const handleEdit = (nhom: NhomCauHoi) => {
    setEditingNhom(nhom);
    setFormData({
      tieuDe: nhom.tieuDe || "",
      noiDung: nhom.noiDung || "",
      urlHinhAnh: nhom.urlHinhAnh || "",
      urlAmThanh: nhom.urlAmThanh || "",
      soLuongCauHoi: nhom.soLuongCauHoi || 0,
      capDo: nhom.capDo ?? 0,
      doKho: nhom.doKho ?? 0,
    });
    setImageFile(null);
    setAudioFile(null);
    setImagePreview(nhom.urlHinhAnh || "");
    setAudioPreview(nhom.urlAmThanh || "");
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
    };
    
    if (editingNhom) {
      const response = await updateNhomCauHoi(editingNhom.id!, submitData);
      setMessage(response.message || "");
      if (response.success) {
        loadNhomCauHois();
        setShowModal(false);
        setTimeout(() => setMessage(""), 3000);
      }
    } else {
      const response = await createNhomCauHoi(submitData);
      setMessage(response.message || "");
      if (response.success) {
        loadNhomCauHois();
        setShowModal(false);
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa nhóm câu hỏi này?")) {
      const response = await deleteNhomCauHoi(id);
      setMessage(response.message || "");
      if (response.success) {
        loadNhomCauHois();
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const handleClone = async (id: string) => {
    const response = await cloneNhomCauHoi(id);
    setMessage(response.message || "");
    if (response.success) {
      loadNhomCauHois();
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const handleViewQuestions = async (nhom: NhomCauHoi) => {
    setSelectedNhom(nhom);
    const response = await getCauHoisInNhom(nhom.id!);
    if (response.success && Array.isArray(response.data)) {
      setNhomQuestions(response.data);
    }
    setShowQuestionsModal(true);
  };

  const handleAddQuestion = async (cauHoiId: string) => {
    if (!selectedNhom) return;
    
    const response = await addCauHoiToNhom(selectedNhom.id!, cauHoiId, nhomQuestions.length + 1);
    setMessage(response.message || "");
    if (response.success) {
      handleViewQuestions(selectedNhom);
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const handleRemoveQuestion = async (cauHoiId: string) => {
    if (!selectedNhom) return;
    
    const response = await removeCauHoiFromNhom(selectedNhom.id!, cauHoiId);
    setMessage(response.message || "");
    if (response.success) {
      handleViewQuestions(selectedNhom);
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const handleExportPDF = async (id: string) => {
    const response = await exportNhomCauHoiToPDF(id);
    if (response.success && response.data) {
      const blob = response.data as Blob;
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `NhomCauHoi_${id}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } else {
      setMessage(response.message || "Xuất PDF thất bại");
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const getCapDoText = (capDo?: CapDo) => {
    switch (capDo) {
      case 0: return "A1";
      case 1: return "A2";
      case 2: return "B1";
      case 3: return "B2";
      case 4: return "C1";
      case 5: return "C2";
      default: return "—";
    }
  };

  const getDoKhoText = (doKho?: DoKho) => {
    switch (doKho) {
      case 0: return "Dễ";
      case 1: return "Trung bình";
      case 2: return "Khó";
      default: return "—";
    }
  };

  const getDoKhoColor = (doKho?: DoKho) => {
    switch (doKho) {
      case 0: return "bg-green-100 text-green-800";
      case 1: return "bg-yellow-100 text-yellow-800";
      case 2: return "bg-red-100 text-red-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const filteredNhoms = nhomCauHois.filter(n => 
    n.tieuDe?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    n.noiDung?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {message && (
        <div className={`${message.includes("thất bại") || message.includes("Lỗi") ? "bg-red-100 border-red-400 text-red-700" : "bg-green-100 border-green-400 text-green-700"} border px-4 py-3 rounded-lg`}>
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div className="flex items-center space-x-4">
          <a 
            href="/admin/cau-hoi"
            className="bg-gray-100 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-200 transition-colors flex items-center space-x-2"
          >
            <ArrowLeft className="w-5 h-5" />
            <span>Về Câu hỏi</span>
          </a>
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Nhóm câu hỏi (Reading Comprehension)</h1>
            <p className="text-gray-600 mt-1">Quản lý nhóm câu hỏi đọc hiểu</p>
          </div>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm nhóm câu hỏi</span>
        </button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <p className="text-blue-700 text-xs font-medium mb-1">Tổng số nhóm</p>
          <p className="text-2xl font-bold text-blue-900">{filteredNhoms.length}</p>
        </div>
        <div className="bg-green-50 border border-green-200 rounded-lg p-4">
          <p className="text-green-700 text-xs font-medium mb-1">Câu hỏi Dễ</p>
          <p className="text-2xl font-bold text-green-900">{filteredNhoms.filter(n => n.doKho === 0).length}</p>
        </div>
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
          <p className="text-yellow-700 text-xs font-medium mb-1">Trung bình</p>
          <p className="text-2xl font-bold text-yellow-900">{filteredNhoms.filter(n => n.doKho === 1).length}</p>
        </div>
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <p className="text-red-700 text-xs font-medium mb-1">Khó</p>
          <p className="text-2xl font-bold text-red-900">{filteredNhoms.filter(n => n.doKho === 2).length}</p>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Tìm kiếm nhóm câu hỏi..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>
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

      {/* Grid View */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {loading ? (
          <div className="col-span-full p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : filteredNhoms.length === 0 ? (
          <div className="col-span-full p-8 text-center">
            <FileText className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Không tìm thấy nhóm câu hỏi nào</p>
          </div>
        ) : (
          filteredNhoms.map((nhom) => (
            <div key={nhom.id} className="bg-white rounded-xl shadow-sm border border-gray-200 hover:shadow-lg transition-shadow">
              <div className="p-6">
                <div className="flex items-start justify-between mb-4">
                  <h3 className="text-lg font-bold text-gray-900 line-clamp-2">{nhom.tieuDe || "Chưa có tiêu đề"}</h3>
                  <div className="flex items-center space-x-1">
                    <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getDoKhoColor(nhom.doKho)}`}>
                      {getDoKhoText(nhom.doKho)}
                    </span>
                  </div>
                </div>
                
                {nhom.urlHinhAnh && (
                  <img 
                    src={nhom.urlHinhAnh} 
                    alt={nhom.tieuDe}
                    className="w-full h-40 object-cover rounded-lg mb-4"
                  />
                )}
                
                <p className="text-sm text-gray-600 line-clamp-3 mb-4">
                  {nhom.noiDung || "Chưa có nội dung"}
                </p>
                
                <div className="flex items-center justify-between text-sm text-gray-500 mb-4">
                  <span className="flex items-center">
                    <List className="w-4 h-4 mr-1" />
                    {nhom.soLuongCauHoi} câu hỏi
                  </span>
                  <span className="font-semibold text-gray-900">
                    {getCapDoText(nhom.capDo)}
                  </span>
                </div>
                
                {nhom.urlAmThanh && (
                  <div className="mb-4">
                    <audio controls className="w-full">
                      <source src={nhom.urlAmThanh} type="audio/mpeg" />
                    </audio>
                  </div>
                )}
                
                <div className="flex items-center space-x-2">
                  <button
                    onClick={() => handleViewQuestions(nhom)}
                    className="flex-1 bg-blue-600 text-white px-3 py-2 rounded-lg hover:bg-blue-700 transition-colors text-sm flex items-center justify-center space-x-1"
                  >
                    <List className="w-4 h-4" />
                    <span>Xem câu hỏi</span>
                  </button>
                  <button
                    onClick={() => handleClone(nhom.id!)}
                    className="bg-green-600 text-white p-2 rounded-lg hover:bg-green-700 transition-colors"
                    title="Sao chép"
                  >
                    <Copy className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleEdit(nhom)}
                    className="bg-gray-900 text-white p-2 rounded-lg hover:bg-gray-800 transition-colors"
                    title="Chỉnh sửa"
                  >
                    <Edit className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleDelete(nhom.id!)}
                    className="bg-red-600 text-white p-2 rounded-lg hover:bg-red-700 transition-colors"
                    title="Xóa"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Create/Edit Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-3xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingNhom ? "Chỉnh sửa nhóm câu hỏi" : "Thêm nhóm câu hỏi mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>
              
              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tiêu đề *</label>
                  <input
                    type="text"
                    required
                    value={formData.tieuDe}
                    onChange={(e) => setFormData({...formData, tieuDe: e.target.value})}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="IELTS Reading - Climate Change"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Nội dung đoạn văn *</label>
                  <textarea
                    required
                    rows={6}
                    value={formData.noiDung}
                    onChange={(e) => setFormData({...formData, noiDung: e.target.value})}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="Nhập nội dung đoạn văn đọc hiểu..."
                  />
                </div>
                
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Cấp độ *</label>
                    <select
                      required
                      value={formData.capDo}
                      onChange={(e) => setFormData({...formData, capDo: parseInt(e.target.value) as CapDo})}
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
                    <label className="block text-sm font-medium text-gray-700 mb-2">Độ khó *</label>
                    <select
                      required
                      value={formData.doKho}
                      onChange={(e) => setFormData({...formData, doKho: parseInt(e.target.value) as DoKho})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value={0}>Dễ</option>
                      <option value={1}>Trung bình</option>
                      <option value={2}>Khó</option>
                    </select>
                  </div>
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Số lượng câu hỏi</label>
                  <input
                    type="number"
                    min="0"
                    value={formData.soLuongCauHoi}
                    onChange={(e) => setFormData({...formData, soLuongCauHoi: parseInt(e.target.value) || 0})}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Hình ảnh</label>
                  <input
                    type="file"
                    accept="image/*"
                    onChange={(e) => {
                      const file = e.target.files?.[0];
                      if (file) {
                        setImageFile(file);
                        const reader = new FileReader();
                        reader.onloadend = () => setImagePreview(reader.result as string);
                        reader.readAsDataURL(file);
                      }
                    }}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                  {imagePreview && (
                    <img src={imagePreview} alt="Preview" className="mt-2 w-full h-40 object-cover rounded-lg" />
                  )}
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">File âm thanh</label>
                  <input
                    type="file"
                    accept="audio/*"
                    onChange={(e) => {
                      const file = e.target.files?.[0];
                      if (file) {
                        setAudioFile(file);
                        setAudioPreview(URL.createObjectURL(file));
                      }
                    }}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                  {audioPreview && (
                    <audio controls className="mt-2 w-full">
                      <source src={audioPreview} type="audio/mpeg" />
                    </audio>
                  )}
                </div>
                
                <div className="flex space-x-3 pt-4">
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
                      editingNhom ? "Cập nhật" : "Thêm mới"
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

      {/* Questions Modal */}
      {showQuestionsModal && selectedNhom && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-6xl w-full max-h-[90vh] flex flex-col">
            {/* Header */}
            <div className="p-6 border-b border-gray-200">
              <div className="flex justify-between items-start">
                <div className="flex-1">
                  <h2 className="text-2xl font-bold text-gray-900 mb-2">
                    {selectedNhom.tieuDe}
                  </h2>
                  <div className="flex items-center space-x-4 text-sm text-gray-600">
                    <span className="flex items-center">
                      <List className="w-4 h-4 mr-1" />
                      {nhomQuestions.length} câu hỏi
                    </span>
                    <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-semibold">
                      {getCapDoText(selectedNhom.capDo)}
                    </span>
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getDoKhoColor(selectedNhom.doKho)}`}>
                      {getDoKhoText(selectedNhom.doKho)}
                    </span>
                  </div>
                </div>
                <button 
                  onClick={() => setShowQuestionsModal(false)} 
                  className="text-gray-500 hover:text-gray-700 p-2 rounded-lg hover:bg-gray-100"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>
            </div>
            
            {/* Content - 2 columns layout */}
            <div className="flex-1 overflow-hidden flex">
              {/* Left side - Questions in group */}
              <div className="w-1/2 border-r border-gray-200 flex flex-col">
                <div className="p-6 border-b border-gray-200 bg-green-50">
                  <h3 className="font-bold text-gray-900 text-lg flex items-center">
                    <span className="bg-green-600 text-white w-8 h-8 rounded-full flex items-center justify-center text-sm mr-2">
                      {nhomQuestions.length}
                    </span>
                    Câu hỏi trong nhóm
                  </h3>
                </div>
                
                <div className="flex-1 overflow-y-auto p-6">
                  {nhomQuestions.length === 0 ? (
                    <div className="text-center py-12">
                      <FileText className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                      <p className="text-gray-500">Chưa có câu hỏi nào trong nhóm</p>
                      <p className="text-sm text-gray-400 mt-2">Thêm câu hỏi từ cột bên phải</p>
                    </div>
                  ) : (
                    <div className="space-y-3">
                      {nhomQuestions.map((q, index) => (
                        <div key={q.id} className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow">
                          <div className="flex items-start justify-between">
                            <div className="flex-1">
                              <div className="flex items-start space-x-3">
                                <span className="flex-shrink-0 w-6 h-6 bg-gray-900 text-white rounded-full flex items-center justify-center text-xs font-bold">
                                  {index + 1}
                                </span>
                                <div className="flex-1">
                                  <p className="text-sm font-medium text-gray-900 mb-2">
                                    {q.noiDung}
                                  </p>
                                  {q.dapAnDung && (
                                    <div className="bg-green-50 border border-green-200 rounded px-3 py-1.5 inline-block">
                                      <span className="text-xs font-semibold text-green-700">Đáp án: </span>
                                      <span className="text-xs text-green-900">{q.dapAnDung}</span>
                                    </div>
                                  )}
                                </div>
                              </div>
                            </div>
                            <button
                              onClick={() => handleRemoveQuestion(q.id!)}
                              className="ml-3 text-red-600 hover:text-white hover:bg-red-600 p-2 rounded-lg transition-colors"
                              title="Xóa câu hỏi khỏi nhóm"
                            >
                              <Trash2 className="w-4 h-4" />
                            </button>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
              
              {/* Right side - Available questions to add */}
              <div className="w-1/2 flex flex-col">
                <div className="p-6 border-b border-gray-200 bg-blue-50">
                  <h3 className="font-bold text-gray-900 text-lg flex items-center">
                    <Plus className="w-5 h-5 mr-2 text-blue-600" />
                    Thêm câu hỏi vào nhóm
                  </h3>
                  <p className="text-sm text-gray-600 mt-1">
                    {allCauHois.filter(cq => !nhomQuestions.some(nq => nq.id === cq.id)).length} câu hỏi có sẵn
                  </p>
                </div>
                
                <div className="flex-1 overflow-y-auto p-6">
                  {allCauHois.filter(cq => !nhomQuestions.some(nq => nq.id === cq.id)).length === 0 ? (
                    <div className="text-center py-12">
                      <FileQuestion className="w-16 h-16 text-gray-300 mx-auto mb-4" />
                      <p className="text-gray-500">Không còn câu hỏi nào để thêm</p>
                      <p className="text-sm text-gray-400 mt-2">Tất cả câu hỏi đã được thêm vào nhóm</p>
                    </div>
                  ) : (
                    <div className="space-y-3">
                      {allCauHois
                        .filter(cq => !nhomQuestions.some(nq => nq.id === cq.id))
                        .map((cq) => (
                          <div key={cq.id} className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md hover:border-blue-300 transition-all">
                            <div className="flex items-start justify-between">
                              <div className="flex-1 pr-4">
                                <p className="text-sm text-gray-900 mb-2 line-clamp-2">{cq.noiDung}</p>
                                {cq.dapAnDung && (
                                  <p className="text-xs text-gray-500">✓ {cq.dapAnDung}</p>
                                )}
                              </div>
                              <button
                                onClick={() => handleAddQuestion(cq.id!)}
                                className="flex-shrink-0 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors text-sm font-medium flex items-center space-x-1"
                              >
                                <Plus className="w-4 h-4" />
                                <span>Thêm</span>
                              </button>
                            </div>
                          </div>
                        ))}
                    </div>
                  )}
                </div>
              </div>
            </div>
            
            {/* Footer */}
            <div className="p-6 border-t border-gray-200 bg-gray-50">
              <div className="flex justify-between items-center">
                <div className="text-sm text-gray-600">
                  💡 <strong>Tip:</strong> Click nút "Thêm" bên phải để thêm câu hỏi vào nhóm
                </div>
                <button
                  onClick={() => setShowQuestionsModal(false)}
                  className="bg-gray-900 text-white px-6 py-2 rounded-lg hover:bg-gray-800 transition-colors"
                >
                  Đóng
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
