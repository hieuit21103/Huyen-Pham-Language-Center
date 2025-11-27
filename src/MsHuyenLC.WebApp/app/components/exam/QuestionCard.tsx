import { useState, useEffect } from "react";
import type { CauHoi, DapAnCauHoi } from "~/types";
import { KyNang } from "~/types";

interface QuestionCardProps {
  question: CauHoi;
  questionNumber: number;
  selectedAnswer?: string;
  onAnswerSelect: (questionId: string, answer: string) => void;
  showResult?: boolean;
  correctAnswer?: string;
}

export function QuestionCard({
  question,
  questionNumber,
  selectedAnswer,
  onAnswerSelect,
  showResult = false,
  correctAnswer,
}: QuestionCardProps) {
  const [essayAnswer, setEssayAnswer] = useState(selectedAnswer || "");

  useEffect(() => {
    if (selectedAnswer !== undefined) {
      setEssayAnswer(selectedAnswer);
    }
  }, [selectedAnswer]);

  const handleAnswerChange = (answer: string) => {
    if (!showResult && question.id) {
      onAnswerSelect(question.id, answer);
    }
  };

  const handleEssayChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const value = e.target.value;
    setEssayAnswer(value);
    if (question.id) {
      onAnswerSelect(question.id, value);
    }
  };

  // Check if this is an essay question (KyNang.Viet = 4)
  const isEssayQuestion = question.kyNang === 4;

  const getAnswerClassName = (answer: DapAnCauHoi) => {
    if (!showResult) {
      return selectedAnswer === answer.nhan
        ? "border-blue-500 bg-blue-50"
        : "border-gray-200 hover:border-blue-300";
    }

    // Show result mode
    const isSelected = selectedAnswer === answer.nhan;
    const isCorrect = answer.dung || correctAnswer === answer.nhan;

    if (isCorrect) {
      return "border-green-500 bg-green-50";
    }
    if (isSelected && !isCorrect) {
      return "border-red-500 bg-red-50";
    }
    return "border-gray-200";
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6 mb-6">
      {/* Question Header */}
      <div className="flex items-start mb-4">
        <span className="inline-flex items-center justify-center w-8 h-8 rounded-full bg-blue-500 text-white font-semibold mr-3 flex-shrink-0">
          {questionNumber}
        </span>
      </div>

      {/* Question Content */}
      <div className="mb-4">
        {question.urlAmThanh && (
          <div className="mb-4">
            <audio controls className="w-full">
              <source src={question.urlAmThanh} type="audio/mpeg" />
              Trình duyệt không hỗ trợ audio.
            </audio>
          </div>
        )}

        {question.urlHinh && (
          <div className="mb-4">
            <img
              src={question.urlHinh}
              alt="Question"
              className="max-w-full h-auto rounded"
            />
          </div>
        )}

        {question.loiThoai && (
          <div className="mb-4 p-4 bg-gray-50 rounded border-l-4 border-blue-500">
            <p className="text-sm text-gray-700 whitespace-pre-wrap">
              {question.loiThoai}
            </p>
          </div>
        )}

        <p className="text-gray-800 text-lg font-medium">
          {question.noiDungCauHoi}
        </p>
      </div>

      {/* Answer Options */}
      {isEssayQuestion ? (
        <div className="space-y-3">
          <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-pink-100 text-pink-700">
            Câu hỏi tự luận
          </span>
          <textarea
            value={essayAnswer}
            onChange={handleEssayChange}
            disabled={showResult}
            placeholder="Nhập câu trả lời của bạn..."
            className="w-full min-h-[150px] p-4 border-2 border-gray-300 rounded-lg focus:border-blue-500 focus:ring-2 focus:ring-blue-200 disabled:bg-gray-100 disabled:cursor-not-allowed"
            rows={6}
          />
          <div className="flex justify-between text-sm text-gray-500">
            <span>Số ký tự: {essayAnswer.length}</span>
          </div>
        </div>
      ) : (
        <div className="space-y-3">
          {question.cacDapAn?.map((answer) => (
            <div
              key={answer.id}
              className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${getAnswerClassName(
                answer
              )}`}
              onClick={() => answer.nhan && handleAnswerChange(answer.nhan)}
            >
              <label className="flex items-start cursor-pointer">
                <input
                  type="radio"
                  name={`question-${question.id}`}
                  value={answer.nhan}
                  checked={selectedAnswer === answer.nhan}
                  onChange={(e) => handleAnswerChange(e.target.value)}
                  disabled={showResult}
                  className="mt-1 mr-3"
                />
                <div className="flex-1">
                  <span className="font-semibold mr-2">{answer.nhan}.</span>
                  <span className="text-gray-700">{answer.noiDung}</span>

                  {showResult && answer.giaiThich && (
                    <div className="mt-2 p-3 bg-blue-50 rounded text-sm text-gray-600">
                      <strong>Giải thích:</strong> {answer.giaiThich}
                    </div>
                  )}
                </div>
              </label>
            </div>
          ))}
        </div>
      )}

      {/* Result Indicator */}
      {showResult && !isEssayQuestion && (
        <div className="mt-4 p-4 rounded-lg">
          {selectedAnswer === correctAnswer ? (
            <div className="flex items-center text-green-700">
              <svg
                className="w-6 h-6 mr-2"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fillRule="evenodd"
                  d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                  clipRule="evenodd"
                />
              </svg>
              <span className="font-semibold">Chính xác!</span>
            </div>
          ) : (
            <div className="flex items-center text-red-700">
              <svg
                className="w-6 h-6 mr-2"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fillRule="evenodd"
                  d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                  clipRule="evenodd"
                />
              </svg>
              <span className="font-semibold">
                Sai. Đáp án đúng: {correctAnswer}
              </span>
            </div>
          )}
        </div>
      )}

      {/* Essay Result Indicator */}
      {showResult && isEssayQuestion && (
        <div className="mt-4 p-4 rounded-lg bg-blue-50 border border-blue-200">
          <div className="flex items-start space-x-2">
            <svg
              className="w-6 h-6 text-blue-600 flex-shrink-0 mt-1"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fillRule="evenodd"
                d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                clipRule="evenodd"
              />
            </svg>
            <div className="flex-1">
              <p className="font-semibold text-blue-900 mb-1">
                Câu hỏi tự luận - Chờ chấm điểm
              </p>
              <p className="text-sm text-blue-700">
                Câu trả lời của bạn đã được ghi nhận và sẽ được giáo viên chấm điểm thủ công.
              </p>
              {essayAnswer && (
                <div className="mt-3 p-3 bg-white rounded border border-blue-200">
                  <p className="text-sm font-medium text-gray-700 mb-1">Câu trả lời của bạn:</p>
                  <p className="text-sm text-gray-600 whitespace-pre-wrap">{essayAnswer}</p>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
