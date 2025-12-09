import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router";
import { getPhienLamBaiDetails } from "~/apis/PhienLamBai";
import { CheckCircle2, XCircle, Clock, Award, ChevronLeft, Eye } from "lucide-react";
import type { CauTraLoi, PhienLamBai } from "~/types";

interface PhienLamBaiResponse {
  phienLamBai: PhienLamBai;
  cauTraLoi: CauTraLoi[];
}

export default function KetQuaThi() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const phienId = searchParams.get("phienId");

  const [loading, setLoading] = useState(true);
  const [result, setResult] = useState<PhienLamBai | any>(null);
  const [answers, setAnswers] = useState<CauTraLoi[]>([]);
  const [showReview, setShowReview] = useState(false);

  useEffect(() => {
    loadResult();
  }, [phienId]);

  const loadResult = async () => {
    if (!phienId) {
      navigate("/luyen-thi");
      return;
    }

    setLoading(true);
    const response = await getPhienLamBaiDetails(phienId);
    if (response.success) {
      setResult(response.data);
      setAnswers(response.data.cauTraLoi || []);
    }
    setLoading(false);
  };

  const formatTime = (timeString?: string) => {
    if (!timeString) return "00:00";
    return timeString;
  };

  const getScoreColor = (score?: number) => {
    if (!score) return "text-gray-600";
    if (score >= 8) return "text-green-600";
    if (score >= 6.5) return "text-blue-600";
    if (score >= 5) return "text-yellow-600";
    return "text-red-600";
  };

  const getScoreLabel = (score?: number) => {
    if (!score) return "Chưa chấm";
    if (score >= 8) return "Xuất sắc";
    if (score >= 6.5) return "Khá";
    if (score >= 5) return "Trung bình";
    return "Yếu";
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
          <p className="text-gray-600">Đang tải kết quả...</p>
        </div>
      </div>
    );
  }

  if (!result) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <XCircle className="w-16 h-16 text-red-500 mx-auto mb-4" />
          <p className="text-gray-600">Không tìm thấy kết quả thi</p>
          <button
            onClick={() => navigate("/luyen-thi")}
            className="mt-4 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Quay lại
          </button>
        </div>
      </div>
    );
  }

  const totalQuestions = answers.length;
  const correctAnswers = result.soCauDung || answers.filter((ct: any) => ct.dung).length;
  const wrongAnswers = totalQuestions - correctAnswers;
  const score = result.diem || 0;
  const timeSpent = result.thoiGianLam || "00:00:00";
  const ngayLam = result.ngayLam || "";

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-5xl mx-auto px-4">
        {/* Header */}
        <div className="bg-white rounded-lg shadow-lg p-8 mb-6">
          <div className="text-center mb-8">
            <div className={`inline-flex items-center justify-center w-24 h-24 rounded-full mb-4 ${
              score >= 5 ? 'bg-green-100' : 'bg-red-100'
            }`}>
              {score >= 5 ? (
                <CheckCircle2 className={`w-12 h-12 ${getScoreColor(score)}`} />
              ) : (
                <XCircle className="w-12 h-12 text-red-600" />
              )}
            </div>
            <h1 className="text-3xl font-bold text-gray-800 mb-2">
              Kết quả bài thi
            </h1>
            {ngayLam && (
              <p className="text-gray-600">Ngày làm: {new Date(ngayLam).toLocaleDateString("vi-VN")}</p>
            )}
          </div>

          {/* Score Card */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
            <div className="bg-gradient-to-br from-blue-50 to-blue-100 rounded-lg p-6 text-center">
              <Award className="w-8 h-8 text-blue-600 mx-auto mb-2" />
              <div className={`text-4xl font-bold ${getScoreColor(score)} mb-1`}>
                {score.toFixed(1)}
              </div>
              <div className="text-sm text-gray-600">{getScoreLabel(score)}</div>
            </div>

            <div className="bg-gradient-to-br from-green-50 to-green-100 rounded-lg p-6 text-center">
              <CheckCircle2 className="w-8 h-8 text-green-600 mx-auto mb-2" />
              <div className="text-4xl font-bold text-green-600 mb-1">
                {correctAnswers}
              </div>
              <div className="text-sm text-gray-600">Câu đúng</div>
            </div>

            <div className="bg-gradient-to-br from-red-50 to-red-100 rounded-lg p-6 text-center">
              <XCircle className="w-8 h-8 text-red-600 mx-auto mb-2" />
              <div className="text-4xl font-bold text-red-600 mb-1">
                {wrongAnswers}
              </div>
              <div className="text-sm text-gray-600">Câu sai</div>
            </div>

            <div className="bg-gradient-to-br from-purple-50 to-purple-100 rounded-lg p-6 text-center">
              <Clock className="w-8 h-8 text-purple-600 mx-auto mb-2" />
              <div className="text-4xl font-bold text-purple-600 mb-1">
                {timeSpent}
              </div>
              <div className="text-sm text-gray-600">Thời gian</div>
            </div>
          </div>

          {/* Progress Bar */}
          <div className="mb-6">
            <div className="flex justify-between text-sm text-gray-600 mb-2">
              <span>Tỷ lệ đúng</span>
              <span>{totalQuestions > 0 ? ((correctAnswers / totalQuestions) * 100).toFixed(1) : 0}%</span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
              <div 
                className="bg-gradient-to-r from-green-500 to-green-600 h-full rounded-full transition-all duration-500"
                style={{ width: `${totalQuestions > 0 ? (correctAnswers / totalQuestions) * 100 : 0}%` }}
              />
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex flex-wrap gap-4 justify-center">
            <button
              onClick={() => setShowReview(true)}
              className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <Eye className="w-5 h-5" />
              Xem chi tiết đáp án
            </button>
            <button
              onClick={() => navigate("/luyen-thi")}
              className="flex items-center gap-2 px-6 py-3 bg-gray-600 text-white rounded-lg hover:bg-gray-700 transition-colors"
            >
              <ChevronLeft className="w-5 h-5" />
              Quay lại danh sách
            </button>
          </div>
        </div>

        {/* Detailed Review Section */}
        {showReview && answers && answers.length > 0 && (
          <div className="bg-white rounded-lg shadow-lg p-8">
            <h2 className="text-2xl font-bold text-gray-800 mb-6">Chi tiết bài làm</h2>
            
            <div className="space-y-6">
              {answers.map((answer: any, index: number) => {
                const isCorrect = answer.dung;
                
                return (
                  <div 
                    key={answer.id || index}
                    className={`border-2 rounded-lg p-6 ${
                      isCorrect ? 'border-green-200 bg-green-50' : 'border-red-200 bg-red-50'
                    }`}
                  >
                    <div className="flex items-start gap-4">
                      <div className={`flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center ${
                        isCorrect ? 'bg-green-500' : 'bg-red-500'
                      }`}>
                        {isCorrect ? (
                          <CheckCircle2 className="w-5 h-5 text-white" />
                        ) : (
                          <XCircle className="w-5 h-5 text-white" />
                        )}
                      </div>
                      
                      <div className="flex-1">
                        <div className="flex items-center gap-2 mb-3">
                          <span className="font-semibold text-gray-700">Câu {index + 1}</span>
                          <span className={`px-2 py-1 rounded text-xs font-medium ${
                            isCorrect ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                          }`}>
                            {isCorrect ? 'Đúng' : 'Sai'}
                          </span>
                        </div>
                        <div className="mb-4">
                          <p className="text-gray-800 font-medium">{answer.noiDungCauHoi}</p>
                        </div>

                        {/* Hiển thị tất cả các đáp án */}
                        {answer.cacDapAn && answer.cacDapAn.length > 0 ? (
                          <div className="space-y-2">
                            {answer.cacDapAn.map((dapAn: any, idx: number) => {
                              const isDapAnDung = dapAn.dung;
                              const isUserAnswer = answer.noiDung === dapAn.nhan;
                              
                              return (
                                <div
                                  key={idx}
                                  className={`p-3 rounded-lg border-2 ${
                                    isDapAnDung && isUserAnswer
                                      ? 'border-green-500 bg-green-50'
                                      : isDapAnDung
                                      ? 'border-green-400 bg-green-50'
                                      : isUserAnswer
                                      ? 'border-red-400 bg-red-50'
                                      : 'border-gray-200 bg-white'
                                  }`}
                                >
                                  <div className="flex items-start gap-2">
                                    <span className="font-bold text-gray-700">{dapAn.nhan}.</span>
                                    <div className="flex-1">
                                      <span className="text-gray-800">{dapAn.noiDung}</span>
                                      <div className="flex gap-2 mt-1">
                                        {isDapAnDung && (
                                          <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-green-100 text-green-800">
                                            <CheckCircle2 className="w-3 h-3 mr-1" />
                                            Đáp án đúng
                                          </span>
                                        )}
                                        {isUserAnswer && (
                                          <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium ${
                                            isDapAnDung ? 'bg-blue-100 text-blue-800' : 'bg-red-100 text-red-800'
                                          }`}>
                                            Bạn đã chọn
                                          </span>
                                        )}
                                      </div>
                                      {isDapAnDung && dapAn.giaiThich && (
                                        <div className="mt-2 p-2 bg-white rounded border border-green-200">
                                          <p className="text-xs text-gray-700">
                                            <span className="font-semibold text-green-700">Giải thích: </span>
                                            {dapAn.giaiThich}
                                          </p>
                                        </div>
                                      )}
                                    </div>
                                  </div>
                                </div>
                              );
                            })}
                          </div>
                        ) : (
                          <div className="space-y-3">
                            <div>
                              <span className="text-sm font-medium text-gray-600">Câu trả lời của bạn: </span>
                              <span className={`text-sm font-semibold ${
                                isCorrect ? 'text-green-700' : 'text-red-700'
                              }`}>
                                {answer.noiDung || "Chưa trả lời"}
                              </span>
                            </div>

                            {!isCorrect && answer.dapAnDung && (
                              <div>
                                <span className="text-sm font-medium text-gray-600">Đáp án đúng: </span>
                                <span className="text-sm font-semibold text-green-700">
                                  {answer.dapAnDung}
                                </span>
                              </div>
                            )}

                            {answer.giaiThich && (
                              <div className="mt-3 p-3 bg-white rounded border border-gray-200">
                                <span className="text-sm font-medium text-gray-600">Giải thích: </span>
                                <p className="text-sm text-gray-700 mt-1">{answer.giaiThich}</p>
                              </div>
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
