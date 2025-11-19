import { useState, useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router";
import { AlertCircle, Send, Loader2, Flag, CheckCircle, FileText, BookOpen } from "lucide-react";
import { getDeThi, getDeThiQuestionsGrouped } from "~/apis/DeThi";
import { submitBaiThi } from "~/apis/PhienLamBai";
import { getKyThiById, joinExam } from "~/apis/KyThi";
import { getProfile } from "~/apis/Profile";
import { getByTaiKhoanId } from "~/apis/HocVien";
import type { QuestionsGroupedResponse, KyThi } from "~/types";
import { QuestionList, ExamTimer } from "~/components/exam";

export default function TakeExam() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const deThiId = searchParams.get("deThiId");
  const kyThiId = searchParams.get("kyThiId");

  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [step, setStep] = useState<"loading" | "confirm" | "generating" | "exam">("loading");
  const [kyThi, setKyThi] = useState<KyThi | null>(null);
  const [error, setError] = useState("");
  const [generatedDeThiId, setGeneratedDeThiId] = useState<string>("");
  
  const [examData, setExamData] = useState<QuestionsGroupedResponse | null>(
    null
  );
  const [selectedAnswers, setSelectedAnswers] = useState<Map<string, string>>(
    new Map()
  );
  const [flaggedQuestions, setFlaggedQuestions] = useState<Set<string>>(
    new Set()
  );

  const [submitting, setSubmitting] = useState(false);
  const [showConfirmSubmit, setShowConfirmSubmit] = useState(false);
  const [timeUp, setTimeUp] = useState(false);

  useEffect(() => {
    if (kyThiId) {
      // Flow: Join exam → Generate → Take exam
      loadExamSession();
    } else if (deThiId) {
      // Flow: Take exam directly
      setStep("exam");
      loadExam();
    } else {
      navigate("/luyen-thi");
    }
  }, [deThiId, kyThiId]);

  const loadExamSession = async () => {
    if (!kyThiId) {
      setError("Không tìm thấy kỳ thi");
      setLoading(false);
      return;
    }

    try {
      // Get kỳ thi info
      const kyThiRes = await getKyThiById(kyThiId);
      if (!kyThiRes.success || !kyThiRes.data) {
        setError("Không tìm thấy thông tin kỳ thi");
        setLoading(false);
        return;
      }

      const kyThiData = kyThiRes.data;
      setKyThi(kyThiData);

      // Check time validity
      if (!isExamAvailable(kyThiData)) {
        setError("Chưa đến giờ thi hoặc đã hết giờ làm bài");
        setLoading(false);
        return;
      }

      setStep("confirm");
      setLoading(false);
    } catch (err: any) {
      setError(err.message || "Có lỗi xảy ra");
      setLoading(false);
    }
  };

  const isExamAvailable = (kyThi: KyThi): boolean => {
    if (!kyThi.ngayThi || !kyThi.gioBatDau || !kyThi.gioKetThuc) return false;
    
    const now = new Date();
    const currentDate = now.toISOString().split('T')[0];
    const currentTime = now.toTimeString().substring(0, 5);
    
    const examDate = kyThi.ngayThi.split('T')[0];
    
    if (currentDate !== examDate) return false;
    
    return currentTime >= kyThi.gioBatDau && currentTime <= kyThi.gioKetThuc;
  };

  const handleJoinExam = async () => {
    setGenerating(true);
    setStep("generating");
    setError("");

    try {
      // Get current user info
      const profileRes = await getProfile();
      if (!profileRes.success || !profileRes.data) {
        setError("Không thể xác thực người dùng");
        setGenerating(false);
        setStep("confirm");
        return;
      }

      const hocVienRes = await getByTaiKhoanId(profileRes.data.id!);
      if (!hocVienRes.success || !hocVienRes.data) {
        setError("Không tìm thấy thông tin học viên");
        setGenerating(false);
        setStep("confirm");
        return;
      }

      // Join exam - backend handles: validate, generate exam, create PhienLamBai
      // If already joined, returns existing deThiId
      const joinRes = await joinExam({
        kyThiId: kyThiId!,
        hocVienId: hocVienRes.data.id!
      });

      if (!joinRes.success || !joinRes.data) {
        setError(joinRes.message || "Không thể tham gia kỳ thi");
        setGenerating(false);
        setStep("confirm");
        return;
      }

      const newDeThiId = joinRes.data.deThiId;
      setGeneratedDeThiId(newDeThiId);
      setGenerating(false);

      // Load the generated exam
      setStep("exam");
      await loadExam(newDeThiId);

    } catch (err: any) {
      setError(err.message || "Có lỗi xảy ra khi tham gia kỳ thi");
      setGenerating(false);
      setStep("confirm");
    }
  };

  const loadExam = async (examId?: string) => {
    setLoading(true);
    const targetDeThiId = examId || deThiId;
    
    if (!targetDeThiId) {
      setLoading(false);
      return;
    }

    try {
      const response = await getDeThiQuestionsGrouped(targetDeThiId);
      if (response.success && response.data) {
        setExamData(response.data);

        const savedAnswers = localStorage.getItem(`exam_${targetDeThiId}_answers`);
        if (savedAnswers) {
          setSelectedAnswers(new Map(JSON.parse(savedAnswers)));
        }

        const savedFlags = localStorage.getItem(`exam_${targetDeThiId}_flags`);
        if (savedFlags) {
          setFlaggedQuestions(new Set(JSON.parse(savedFlags)));
        }
      } else {
        console.error("Failed to load exam:", response.message);
      }
    } catch (error) {
      console.error("Error loading exam:", error);
    }
    setLoading(false);
  };

  const handleAnswerSelect = (questionId: string, answer: string) => {
    const newAnswers = new Map(selectedAnswers);
    newAnswers.set(questionId, answer);
    setSelectedAnswers(newAnswers);

    const targetDeThiId = generatedDeThiId || deThiId;
    localStorage.setItem(
      `exam_${targetDeThiId}_answers`,
      JSON.stringify(Array.from(newAnswers.entries()))
    );
  };

  const toggleFlag = (questionId: string) => {
    const newFlags = new Set(flaggedQuestions);
    if (newFlags.has(questionId)) {
      newFlags.delete(questionId);
    } else {
      newFlags.add(questionId);
    }
    setFlaggedQuestions(newFlags);

    // Save to localStorage
    const targetDeThiId = generatedDeThiId || deThiId;
    localStorage.setItem(
      `exam_${targetDeThiId}_flags`,
      JSON.stringify(Array.from(newFlags))
    );
  };

  const handleTimeUp = () => {
    setTimeUp(true);
    handleSubmit(true);
  };

  const handleSubmit = async (autoSubmit = false) => {
    const targetDeThiId = generatedDeThiId || deThiId;
    if (!targetDeThiId || !examData) return;

    setSubmitting(true);
    setShowConfirmSubmit(false);

    // Prepare answers
    const cacTraLoi: Record<string, string> = {};

    // Collect all question IDs
    const allQuestionIds: string[] = [];
    examData.groupedQuestions.forEach((item) => {
      if (item.cauHoi.id) allQuestionIds.push(item.cauHoi.id);
    });
    examData.standaloneQuestions.forEach((item) => {
      if (item.cauHoi.id) allQuestionIds.push(item.cauHoi.id);
    });

    allQuestionIds.forEach((qid) => {
      cacTraLoi[qid] = selectedAnswers.get(qid) || "";
    });

    try {
      const response = await submitBaiThi({
        deThiId: targetDeThiId,
        tongCauHoi: examData.totalQuestions,
        cacTraLoi,
        thoiGianLamBai: "00:00:00",
        tuDongCham: true,
      });

      if (response.success) {
        localStorage.removeItem(`exam_${targetDeThiId}_answers`);
        localStorage.removeItem(`exam_${targetDeThiId}_flags`);
        localStorage.removeItem("exam-time-left");

        navigate(`/ket-qua-thi?phienId=${response.data?.phienLamBaiId || ""}`);
      } else {
        alert(`Lỗi nộp bài: ${response.message}`);
        setSubmitting(false);
      }
    } catch (error) {
      console.error("Submit error:", error);
      alert("Có lỗi xảy ra khi nộp bài!");
      setSubmitting(false);
    }
  };

  const getTotalAnswered = () => selectedAnswers.size;
  const getTotalUnanswered = () =>
    examData ? examData.totalQuestions - selectedAnswers.size : 0;

  const formatDate = (dateString?: string) => {
    if (!dateString) return "—";
    return new Date(dateString).toLocaleDateString("vi-VN");
  };

  // Loading initial data
  if (loading && step === "loading") {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-16 h-16 text-blue-600 animate-spin mx-auto mb-4" />
          <p className="text-gray-600 text-lg">Đang tải thông tin kỳ thi...</p>
        </div>
      </div>
    );
  }

  // Error state
  if (error && step === "loading") {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-xl shadow-lg p-8 max-w-md w-full text-center">
          <AlertCircle className="w-16 h-16 text-red-600 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Không thể vào thi</h2>
          <p className="text-gray-600 mb-6">{error}</p>
          <button
            onClick={() => navigate("/dashboard")}
            className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Quay lại Dashboard
          </button>
        </div>
      </div>
    );
  }

  // Generating exam
  if (step === "generating") {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="bg-white rounded-xl shadow-lg p-8 max-w-md w-full text-center">
          <Loader2 className="w-16 h-16 text-blue-600 animate-spin mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Đang chuẩn bị đề thi...</h2>
          <p className="text-gray-600">
            Hệ thống đang tạo đề thi dựa trên cấu hình của kỳ thi.
            <br />Vui lòng đợi trong giây lát...
          </p>
        </div>
      </div>
    );
  }

  // Confirm join exam
  if (step === "confirm" && kyThi) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-xl shadow-lg p-8 max-w-2xl w-full">
          <div className="text-center mb-8">
            <div className="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <FileText className="w-8 h-8 text-blue-600" />
            </div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">
              Xác nhận tham gia kỳ thi
            </h1>
            <p className="text-gray-600">
              Vui lòng xem kỹ thông tin trước khi bắt đầu
            </p>
          </div>

          <div className="bg-gray-50 rounded-lg p-6 mb-6 space-y-4">
            <div className="flex justify-between items-start">
              <span className="text-gray-600 font-medium">Tên kỳ thi:</span>
              <span className="text-gray-900 font-semibold text-right">{kyThi.tenKyThi}</span>
            </div>
            
            <div className="flex justify-between items-start">
              <span className="text-gray-600 font-medium">Lớp học:</span>
              <span className="text-gray-900 font-semibold text-right">{kyThi.lopHoc?.tenLop}</span>
            </div>

            <div className="flex justify-between items-start">
              <span className="text-gray-600 font-medium">Ngày thi:</span>
              <span className="text-gray-900 font-semibold">{formatDate(kyThi.ngayThi)}</span>
            </div>

            <div className="flex justify-between items-start">
              <span className="text-gray-600 font-medium">Giờ thi:</span>
              <span className="text-gray-900 font-semibold">
                {kyThi.gioBatDau} - {kyThi.gioKetThuc}
              </span>
            </div>

            <div className="flex justify-between items-start">
              <span className="text-gray-600 font-medium">Thời lượng:</span>
              <span className="text-gray-900 font-semibold">{kyThi.thoiLuong} phút</span>
            </div>
          </div>

          {error && (
            <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-6 flex items-start">
              <AlertCircle className="w-5 h-5 text-red-600 mr-3 flex-shrink-0 mt-0.5" />
              <p className="text-red-700 text-sm">{error}</p>
            </div>
          )}

          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6">
            <div className="flex items-start">
              <AlertCircle className="w-5 h-5 text-yellow-600 mr-3 flex-shrink-0 mt-0.5" />
              <div className="text-sm text-yellow-800">
                <p className="font-semibold mb-2">Lưu ý quan trọng:</p>
                <ul className="list-disc list-inside space-y-1">
                  <li>Sau khi bắt đầu, thời gian sẽ đếm ngược tự động</li>
                  <li>Đề thi sẽ được tạo ngẫu nhiên dựa trên cấu hình</li>
                  <li>Bạn không thể thoát ra và vào lại</li>
                  <li>Hãy đảm bảo kết nối internet ổn định</li>
                </ul>
              </div>
            </div>
          </div>

          <div className="flex gap-4">
            <button
              onClick={() => navigate("/dashboard")}
              className="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 font-semibold transition-colors"
            >
              Hủy bỏ
            </button>
            <button
              onClick={handleJoinExam}
              disabled={generating}
              className="flex-1 px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 font-semibold transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
            >
              {generating ? (
                <>
                  <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                  Đang xử lý...
                </>
              ) : (
                <>
                  <BookOpen className="w-5 h-5 mr-2" />
                  Xác nhận và bắt đầu
                </>
              )}
            </button>
          </div>
        </div>
      </div>
    );
  }

  // Loading exam after generation or direct access
  if (loading && step === "exam") {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-16 h-16 text-blue-600 animate-spin mx-auto mb-4" />
          <p className="text-gray-600 text-lg">Đang tải đề thi...</p>
        </div>
      </div>
    );
  }

  if (!examData) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <AlertCircle className="w-16 h-16 text-red-600 mx-auto mb-4" />
          <p className="text-gray-900 text-lg font-semibold">
            Không thể tải đề thi
          </p>
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

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white border-b border-gray-200 sticky top-0 z-50 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between flex-wrap gap-4">
            {/* Exam Info */}
            <div>
              <h1 className="text-xl font-bold text-gray-900">
                {examData.deThi.tenDe || "Đề thi"}
              </h1>
              <p className="text-sm text-gray-600">
                Mã đề: {examData.deThi.maDe} • {examData.totalQuestions} câu hỏi
              </p>
            </div>

            {/* Submit Button */}
            <button
              onClick={() => setShowConfirmSubmit(true)}
              disabled={submitting}
              className="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2 font-semibold disabled:opacity-50"
            >
              <Send className="w-5 h-5" />
              <span>Nộp bài</span>
            </button>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Main Content */}
          <div className="lg:col-span-3">
            {/* Timer */}
            <div className="mb-6">
              <ExamTimer
                totalMinutes={examData.deThi.thoiLuongPhut || 60}
                onTimeUp={handleTimeUp}
                isPaused={submitting}
              />
            </div>

            {/* Questions */}
            <QuestionList
              data={examData}
              selectedAnswers={selectedAnswers}
              onAnswerSelect={handleAnswerSelect}
            />
          </div>

          {/* Sidebar */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-md p-6 sticky top-24 space-y-6">
              <h3 className="font-bold text-gray-900 text-lg">Thông tin</h3>

              {/* Progress Stats */}
              <div className="space-y-3">
                <div className="flex justify-between items-center p-3 bg-blue-50 rounded-lg">
                  <span className="text-sm font-medium text-blue-700">
                    Tổng câu hỏi
                  </span>
                  <span className="text-lg font-bold text-blue-900">
                    {examData.totalQuestions}
                  </span>
                </div>

                <div className="flex justify-between items-center p-3 bg-green-50 rounded-lg">
                  <span className="text-sm font-medium text-green-700">
                    Đã trả lời
                  </span>
                  <span className="text-lg font-bold text-green-900">
                    {getTotalAnswered()}
                  </span>
                </div>

                <div className="flex justify-between items-center p-3 bg-orange-50 rounded-lg">
                  <span className="text-sm font-medium text-orange-700">
                    Chưa trả lời
                  </span>
                  <span className="text-lg font-bold text-orange-900">
                    {getTotalUnanswered()}
                  </span>
                </div>

                <div className="flex justify-between items-center p-3 bg-yellow-50 rounded-lg">
                  <span className="text-sm font-medium text-yellow-700">
                    Đánh dấu
                  </span>
                  <span className="text-lg font-bold text-yellow-900">
                    {flaggedQuestions.size}
                  </span>
                </div>
              </div>

              {/* Progress Bar */}
              <div>
                <div className="flex justify-between text-sm mb-2">
                  <span className="text-gray-600">Tiến độ</span>
                  <span className="font-semibold text-gray-900">
                    {Math.round(
                      (getTotalAnswered() / examData.totalQuestions) * 100
                    )}
                    %
                  </span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-3">
                  <div
                    className="bg-blue-600 h-3 rounded-full transition-all duration-300"
                    style={{
                      width: `${
                        (getTotalAnswered() / examData.totalQuestions) * 100
                      }%`,
                    }}
                  />
                </div>
              </div>

              {/* Warning */}
              {getTotalUnanswered() > 0 && (
                <div className="p-3 bg-yellow-50 border border-yellow-200 rounded-lg">
                  <p className="text-sm text-yellow-800">
                    ⚠️ Bạn còn {getTotalUnanswered()} câu chưa trả lời
                  </p>
                </div>
              )}
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
              <h2 className="text-xl font-bold text-gray-900">
                Xác nhận nộp bài
              </h2>
            </div>

            <div className="space-y-3 mb-6">
              <p className="text-gray-700">
                Bạn có chắc chắn muốn nộp bài thi không?
              </p>

              <div className="bg-gray-50 rounded-lg p-4 space-y-2 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Tổng số câu:</span>
                  <span className="font-semibold text-gray-900">
                    {examData.totalQuestions}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Đã trả lời:</span>
                  <span className="font-semibold text-green-600">
                    {getTotalAnswered()}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Chưa trả lời:</span>
                  <span className="font-semibold text-red-600">
                    {getTotalUnanswered()}
                  </span>
                </div>
              </div>

              {getTotalUnanswered() > 0 && (
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3">
                  <p className="text-sm text-yellow-800">
                    ⚠️ Bạn còn {getTotalUnanswered()} câu chưa trả lời!
                  </p>
                </div>
              )}
            </div>

            <div className="flex space-x-3">
              <button
                onClick={() => handleSubmit(false)}
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

      {/* Time Up Modal */}
      {timeUp && (
        <div className="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full p-6 text-center">
            <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <AlertCircle className="w-10 h-10 text-red-600" />
            </div>
            <h2 className="text-2xl font-bold text-gray-900 mb-2">
              Hết giờ làm bài!
            </h2>
            <p className="text-gray-600 mb-4">
              Bài thi của bạn đang được tự động nộp...
            </p>
            <Loader2 className="w-8 h-8 text-blue-600 animate-spin mx-auto" />
          </div>
        </div>
      )}
    </div>
  );
}
