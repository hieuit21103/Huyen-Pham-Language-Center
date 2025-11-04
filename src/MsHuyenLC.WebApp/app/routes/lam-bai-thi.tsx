import { useState, useEffect, useRef } from "react";
import { useSearchParams, useNavigate } from "react-router";
import {
  Clock,
  ChevronLeft,
  ChevronRight,
  CheckCircle,
  AlertCircle,
  Flag,
  Send,
  Volume2,
  Image as ImageIcon,
  Loader2,
  User,
  LogOut,
  LayoutDashboard
} from "lucide-react";
import { getDeThi, getDeThiQuestionsGrouped } from "~/apis/DeThi";
import { submitBaiThi } from "~/apis/PhienLamBai";
import type { DapAnCauHoi, CauHoi, NhomCauHoi as BaseNhomCauHoi } from "~/types";

interface NhomCauHoi extends Omit<BaseNhomCauHoi, 'cacChiTiet'> {
  cauHois?: CauHoi[];
}

interface CauTraLoi {
  cauHoiId: string;
  cauTraLoi: string;
  danhDau?: boolean;
}

export default function LamBaiThi() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const deThiId = searchParams.get('deThiId');

  const [loading, setLoading] = useState(true);
  const [tenDe, setTenDe] = useState("");
  const [thoiGianLamBai, setThoiGianLamBai] = useState(0); // phút
  const [nhomCauHois, setNhomCauHois] = useState<NhomCauHoi[]>([]);
  const [cauHois, setCauHois] = useState<CauHoi[]>([]);
  const [currentNhomIndex, setCurrentNhomIndex] = useState(0);
  const [answers, setAnswers] = useState<Map<string, CauTraLoi>>(new Map());
  const [flaggedQuestions, setFlaggedQuestions] = useState<Set<string>>(new Set());
  const [phienLamBaiId, setPhienLamBaiId] = useState<string>("");

  const [timeRemaining, setTimeRemaining] = useState(0); // giây
  const [isTimeUp, setIsTimeUp] = useState(false);
  const timerRef = useRef<NodeJS.Timeout | null>(null);

  const [submitting, setSubmitting] = useState(false);
  const [showConfirmSubmit, setShowConfirmSubmit] = useState(false);
  const [showWarning, setShowWarning] = useState(false);
  const [showUserMenu, setShowUserMenu] = useState(false);

  useEffect(() => {
    if (!deThiId) {
      navigate('/luyen-thi');
      return;
    }
    loadExam();
  }, [deThiId]);

  useEffect(() => {
    if (deThiId && timeRemaining > 0) {
      localStorage.setItem(`exam_${deThiId}_time`, timeRemaining.toString());
      localStorage.setItem(`exam_${deThiId}_timestamp`, Date.now().toString());
    }
  }, [timeRemaining, deThiId]);

  useEffect(() => {
    if (timeRemaining > 0 && !isTimeUp) {
      timerRef.current = setInterval(() => {
        setTimeRemaining(prev => {
          if (prev <= 1) {
            setIsTimeUp(true);
            handleAutoSubmit();
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    }

    return () => {
      if (timerRef.current) {
        clearInterval(timerRef.current);
      }
    };
  }, [timeRemaining, isTimeUp]);

  const loadExam = async () => {
    setLoading(true);

    try {
      const deThiRes = await getDeThi(deThiId!);
      if (deThiRes.success && deThiRes.data) {
        setTenDe(deThiRes.data.tenDe || "");
        const thoiGian = deThiRes.data.thoiLuongPhut || deThiRes.data.thoiGianLamBai || 60;
        setThoiGianLamBai(thoiGian);

        const savedTime = localStorage.getItem(`exam_${deThiId}_time`);
        const savedTimestamp = localStorage.getItem(`exam_${deThiId}_timestamp`);

        if (savedTime && savedTimestamp) {
          const timePassed = Math.floor((Date.now() - parseInt(savedTimestamp)) / 1000);
          const remainingTime = Math.max(0, parseInt(savedTime) - timePassed);

          if (remainingTime > 0) {
            setTimeRemaining(remainingTime);
          } else {
            setTimeRemaining(0);
            setIsTimeUp(true);
            localStorage.removeItem(`exam_${deThiId}_time`);
            localStorage.removeItem(`exam_${deThiId}_timestamp`);
          }
        } else {
          setTimeRemaining(thoiGian * 60);
        }
      }

      const cauHoiRes = await getDeThiQuestionsGrouped(deThiId!);
      if (cauHoiRes.success && cauHoiRes.data) {
        const responseData = cauHoiRes.data;
        
        if (responseData.nhomCauHois && Array.isArray(responseData.nhomCauHois)) {
          setNhomCauHois(responseData.nhomCauHois);
          
          const allQuestions: CauHoi[] = [];
          responseData.nhomCauHois.forEach((nhom: NhomCauHoi) => {
            if (nhom.cauHois) {
              allQuestions.push(...nhom.cauHois);
            }
          });
          setCauHois(allQuestions);
          
          console.log("Loaded grouped questions:", responseData.nhomCauHois);
          console.log("Total questions:", allQuestions.length);
        }
      }
    } catch (error) {
      console.error("Error loading exam:", error);
    }

    setLoading(false);
  };

  const handleAutoSubmit = async () => {
    if (!deThiId) return;
    setSubmitting(true);

    localStorage.removeItem(`exam_${deThiId}_time`);
    localStorage.removeItem(`exam_${deThiId}_timestamp`);

    const cacTraLoi: Record<string, string> = {};
    
    cauHois.forEach(cauHoi => {
      if (cauHoi.id) {
        const answer = answers.get(cauHoi.id);
        cacTraLoi[cauHoi.id] = answer?.cauTraLoi || "";
      }
    });

    const response = await submitBaiThi({
      tongCauHoi: cauHois.length,
      thoiGianLamBai: (thoiGianLamBai * 60 - timeRemaining).toString(),
      deThiId,
      cacTraLoi,
      tuDongCham: true
    });

    setSubmitting(false);

    if (response.success) {
      navigate(`/ket-qua-thi?phienId=${response.data.phienLamBaiId}`);
    }
  };

  const handleSubmit = async () => {
    if (!deThiId) return;

    const unanswered = cauHois.filter(cq => !answers.has(cq.id!));
    if (unanswered.length > 0 && !showWarning) {
      setShowWarning(true);
      return;
    }

    setSubmitting(true);

    localStorage.removeItem(`exam_${deThiId}_time`);
    localStorage.removeItem(`exam_${deThiId}_timestamp`);

    const cacTraLoi: Record<string, string> = {};
    
    // Thêm tất cả câu hỏi, kể cả câu chưa trả lời
    cauHois.forEach(cauHoi => {
      if (cauHoi.id) {
        const answer = answers.get(cauHoi.id);
        cacTraLoi[cauHoi.id] = answer?.cauTraLoi || ""; // Rỗng nếu chưa trả lời
      }
    });

    const response = await submitBaiThi({
      tongCauHoi: cauHois.length,
      thoiGianLamBai: (thoiGianLamBai * 60 - timeRemaining).toString(),
      deThiId,
      cacTraLoi,
      tuDongCham: true
    });

    setSubmitting(false);
    setShowConfirmSubmit(false);

    if (response.success) {
      navigate(`/ket-qua-thi?phienId=${response.data.phienLamBaiId}`);
    }
  };

  const handleAnswer = async (cauHoiId: string, answer: string) => {
    const newAnswer: CauTraLoi = {
      cauHoiId,
      cauTraLoi: answer,
      danhDau: flaggedQuestions.has(cauHoiId)
    };

    setAnswers(prev => new Map(prev).set(cauHoiId, newAnswer));
  };

  const toggleFlag = (cauHoiId: string) => {
    setFlaggedQuestions(prev => {
      const newSet = new Set(prev);
      if (newSet.has(cauHoiId)) {
        newSet.delete(cauHoiId);
      } else {
        newSet.add(cauHoiId);
      }
      return newSet;
    });
  };

  const formatTime = (seconds: number) => {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;

    if (hours > 0) {
      return `${hours}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }
    return `${minutes}:${secs.toString().padStart(2, '0')}`;
  };

  const getTimeColor = () => {
    const percentage = (timeRemaining / (thoiGianLamBai * 60)) * 100;
    if (percentage > 50) return "text-green-600";
    if (percentage > 20) return "text-yellow-600";
    return "text-red-600";
  };

  const currentNhom = nhomCauHois[currentNhomIndex];
  const totalAnswered = answers.size;
  const totalFlagged = flaggedQuestions.size;

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-16 h-16 text-gray-900 animate-spin mx-auto mb-4" />
          <p className="text-gray-600 text-lg">Đang tải đề thi...</p>
        </div>
      </div>
    );
  }

  if (!currentNhom || nhomCauHois.length === 0) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <AlertCircle className="w-16 h-16 text-red-600 mx-auto mb-4" />
          <p className="text-gray-900 text-lg font-semibold">Không tìm thấy câu hỏi</p>
          <button
            onClick={() => navigate('/luyen-thi')}
            className="mt-4 px-6 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800"
          >
            Quay lại
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header - Fixed */}
      <div className="bg-white border-b border-gray-200 sticky top-0 z-50 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            {/* Timer */}
            <div className="flex items-center space-x-6">
              <div className={`flex items-center space-x-2 px-4 py-2 bg-gray-100 rounded-lg ${getTimeColor()}`}>
                <Clock className="w-5 h-5" />
                <span className="text-xl font-bold font-mono">
                  {formatTime(timeRemaining)}
                </span>
              </div>

              <button
                onClick={() => setShowConfirmSubmit(true)}
                className="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2 font-semibold"
              >
                <Send className="w-5 h-5" />
                <span>Nộp bài</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Question Content */}
          <div className="lg:col-span-3 space-y-6">
            {/* Nội dung chung của nhóm (đoạn văn, audio, hình ảnh) */}
            {(currentNhom.noiDung || currentNhom.urlAmThanh || currentNhom.urlHinhAnh) && (
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 space-y-4">
                <h2 className="text-xl font-bold text-gray-900">{currentNhom.tieuDe}</h2>
                
                {/* Đoạn văn */}
                {currentNhom.noiDung && (
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                    <h3 className="font-semibold text-blue-900 mb-2">Đoạn văn:</h3>
                    <p className="text-gray-800 whitespace-pre-wrap leading-relaxed">{currentNhom.noiDung}</p>
                  </div>
                )}

                {/* Audio */}
                {currentNhom.urlAmThanh && (
                  <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
                    <div className="flex items-center space-x-3 mb-2">
                      <Volume2 className="w-5 h-5 text-purple-600" />
                      <h3 className="font-semibold text-purple-900">Nghe audio:</h3>
                    </div>
                    <audio controls className="w-full">
                      <source src={currentNhom.urlAmThanh} type="audio/mpeg" />
                    </audio>
                  </div>
                )}

                {/* Image */}
                {currentNhom.urlHinhAnh && (
                  <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
                    <img
                      src={currentNhom.urlHinhAnh}
                      alt="Nhóm câu hỏi"
                      className="max-w-full h-auto rounded-lg mx-auto"
                    />
                  </div>
                )}
              </div>
            )}

            {/* Danh sách câu hỏi trong nhóm */}
            {currentNhom.cauHois?.map((cauHoi, idx) => (
              <div 
                key={cauHoi.id} 
                id={`question-${cauHoi.id}`}
                className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 scroll-mt-24"
              >
                {/* Question Header */}
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center space-x-3">
                    <span className="flex items-center justify-center w-10 h-10 bg-gray-900 text-white rounded-full font-bold text-sm">
                      {cauHois.findIndex(q => q.id === cauHoi.id) + 1}
                    </span>
                    <h2 className="text-lg font-semibold text-gray-900">
                      Câu hỏi {cauHois.findIndex(q => q.id === cauHoi.id) + 1}
                    </h2>
                  </div>

                  <button
                    onClick={() => toggleFlag(cauHoi.id!)}
                    className={`p-2 rounded-lg transition-colors ${
                      flaggedQuestions.has(cauHoi.id!)
                        ? 'bg-yellow-100 text-yellow-600'
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                    }`}
                    title="Đánh dấu câu hỏi"
                  >
                    <Flag className="w-5 h-5" />
                  </button>
                </div>

                {/* Question Content */}
                <div className="space-y-4">
                  {/* Question Text */}
                  <div className="text-base text-gray-900 font-medium">
                    {cauHoi.noiDungCauHoi}
                  </div>

                  {/* Answers - Trắc nghiệm */}
                  {cauHoi.loaiCauHoi === 0 && (
                    <div className="space-y-2">
                      {(cauHoi.cacDapAn || []).map((dapAn: DapAnCauHoi, index: number) => {
                        const isSelected = answers.get(cauHoi.id!)?.cauTraLoi === dapAn.nhan;

                        return (
                          <label
                            key={index}
                            className={`flex items-start p-3 border-2 rounded-lg cursor-pointer transition-all ${
                              isSelected
                                ? 'border-gray-900 bg-gray-50'
                                : 'border-gray-200 hover:border-gray-300 bg-white'
                            }`}
                          >
                            <input
                              type="radio"
                              name={`question-${cauHoi.id}`}
                              value={dapAn.nhan}
                              checked={isSelected}
                              onChange={(e) => handleAnswer(cauHoi.id!, e.target.value)}
                              className="mt-1 mr-3 w-4 h-4 text-gray-900 focus:ring-gray-900"
                            />
                            <div className="flex-1">
                              <span className="font-bold text-gray-900 mr-2">{dapAn.nhan}.</span>
                              <span className="text-gray-800">{dapAn.noiDung}</span>
                            </div>
                          </label>
                        );
                      })}
                    </div>
                  )}

                  {/* Answers - Tự luận */}
                  {cauHoi.loaiCauHoi === 1 && (
                    <div className="space-y-2">
                      <label className="block text-sm font-medium text-gray-700">
                        Câu trả lời của bạn:
                      </label>
                      <textarea
                        value={answers.get(cauHoi.id!)?.cauTraLoi || ''}
                        onChange={(e) => handleAnswer(cauHoi.id!, e.target.value)}
                        rows={6}
                        className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:ring-gray-900 resize-none"
                        placeholder="Nhập câu trả lời của bạn..."
                      />
                      <p className="text-xs text-gray-500">
                        {answers.get(cauHoi.id!)?.cauTraLoi?.length || 0} ký tự
                      </p>
                    </div>
                  )}
                </div>
              </div>
            ))}

            {/* Navigation */}
            <div className="flex items-center justify-between bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <button
                onClick={() => setCurrentNhomIndex(Math.max(0, currentNhomIndex - 1))}
                disabled={currentNhomIndex === 0}
                className="flex items-center space-x-2 px-6 py-3 bg-gray-100 text-gray-900 rounded-lg hover:bg-gray-200 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                <ChevronLeft className="w-5 h-5" />
                <span className="font-medium">Trang trước</span>
              </button>

              <span className="text-sm text-gray-600">
                Trang {currentNhomIndex + 1} / {nhomCauHois.length}
              </span>

              <button
                onClick={() => setCurrentNhomIndex(Math.min(nhomCauHois.length - 1, currentNhomIndex + 1))}
                disabled={currentNhomIndex === nhomCauHois.length - 1}
                className="flex items-center space-x-2 px-6 py-3 bg-gray-900 text-white rounded-lg hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                <span className="font-medium">Trang tiếp</span>
                <ChevronRight className="w-5 h-5" />
              </button>
            </div>
          </div>

          {/* Question Navigator Sidebar */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sticky top-24 space-y-4">
              <h3 className="font-bold text-gray-900">Danh sách câu hỏi</h3>

              {/* Stats */}
              <div className="grid grid-cols-2 gap-2">
                <div className="bg-green-50 border border-green-200 rounded-lg p-2 text-center">
                  <p className="text-xs text-green-700 font-medium">Đã trả lời</p>
                  <p className="text-lg font-bold text-green-900">{totalAnswered}</p>
                </div>
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-2 text-center">
                  <p className="text-xs text-yellow-700 font-medium">Đánh dấu</p>
                  <p className="text-lg font-bold text-yellow-900">{totalFlagged}</p>
                </div>
              </div>

              {/* Question Grid cho nhóm hiện tại */}
              <div className="border-t border-gray-200 pt-4">
                <h4 className="text-sm font-semibold text-gray-700 mb-2">
                  Câu hỏi trong nhóm {currentNhomIndex + 1}
                </h4>
                <div className="grid grid-cols-5 gap-2 max-h-64 overflow-y-auto">
                  {currentNhom.cauHois?.map((cq, localIdx) => {
                    const globalIdx = cauHois.findIndex(q => q.id === cq.id);
                    const isAnswered = answers.has(cq.id!);
                    const isFlagged = flaggedQuestions.has(cq.id!);

                    return (
                      <button
                        key={cq.id}
                        onClick={() => {
                          // Scroll đến câu hỏi
                          const element = document.getElementById(`question-${cq.id}`);
                          element?.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        }}
                        className={`relative aspect-square rounded-lg font-bold text-sm transition-all ${
                          isAnswered
                            ? 'bg-green-100 text-green-900 hover:bg-green-200'
                            : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                        }`}
                      >
                        {globalIdx + 1}
                        {isFlagged && (
                          <Flag className="w-3 h-3 absolute -top-1 -right-1 text-yellow-600 fill-yellow-600" />
                        )}
                      </button>
                    );
                  })}
                </div>
              </div>
            </div>
          </div>
          
        </div>
      </div>

      {/* Confirm Submit Modal */}
      {showConfirmSubmit && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full p-6">
            <div className="flex items-center space-x-3 mb-4">
              <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
                <Send className="w-6 h-6 text-green-600" />
              </div>
              <h2 className="text-xl font-bold text-gray-900">Xác nhận nộp bài</h2>
            </div>

            <div className="space-y-3 mb-6">
              <p className="text-gray-700">
                Bạn có chắc chắn muốn nộp bài thi không?
              </p>
              <div className="bg-gray-50 rounded-lg p-4 space-y-2 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Tổng số câu:</span>
                  <span className="font-semibold text-gray-900">{cauHois.length}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Đã trả lời:</span>
                  <span className="font-semibold text-green-600">{totalAnswered}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Chưa trả lời:</span>
                  <span className="font-semibold text-red-600">{cauHois.length - totalAnswered}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Thời gian còn lại:</span>
                  <span className={`font-semibold ${getTimeColor()}`}>
                    {formatTime(timeRemaining)}
                  </span>
                </div>
              </div>
              {cauHois.length - totalAnswered > 0 && (
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3">
                  <p className="text-sm text-yellow-800">
                    ⚠️ Bạn còn {cauHois.length - totalAnswered} câu chưa trả lời!
                  </p>
                </div>
              )}
            </div>

            <div className="flex space-x-3">
              <button
                onClick={handleSubmit}
                disabled={submitting}
                className="flex-1 bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 transition-colors font-semibold disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
              >
                {submitting ? (
                  <>
                    <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                    Đang nộp...
                  </>
                ) : (
                  "Nộp bài"
                )}
              </button>
              <button
                onClick={() => setShowConfirmSubmit(false)}
                disabled={submitting}
                className="flex-1 bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors font-semibold"
              >
                Tiếp tục làm
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
