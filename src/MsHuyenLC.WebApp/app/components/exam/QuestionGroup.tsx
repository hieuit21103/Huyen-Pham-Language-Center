import type { NhomCauHoi, CauHoi } from "~/types";
import { QuestionCard } from "./QuestionCard";

interface QuestionGroupProps {
  group: NhomCauHoi;
  questions: Array<{ cauHoi: CauHoi; thuTu: number }>;
  startQuestionNumber: number;
  selectedAnswers: Map<string, string>;
  onAnswerSelect: (questionId: string, answer: string) => void;
  showResult?: boolean;
  correctAnswers?: Map<string, string>;
}

export function QuestionGroup({
  group,
  questions,
  startQuestionNumber,
  selectedAnswers,
  onAnswerSelect,
  showResult = false,
  correctAnswers,
}: QuestionGroupProps) {
  return (
    <div className="bg-gray-50 rounded-lg p-6 mb-6 border-2 border-gray-200">
      {/* Group Header */}
      <div className="mb-6">
        {group.tieuDe && (
          <h3 className="text-xl font-bold text-gray-800 mb-3">
            {group.tieuDe}
          </h3>
        )}

        <div className="flex gap-2 mb-4">
          <span className="px-3 py-1 text-sm bg-indigo-100 text-indigo-700 rounded-full">
            Nhóm câu hỏi
          </span>
          <span className="px-3 py-1 text-sm bg-gray-200 text-gray-700 rounded-full">
            {questions.length} câu hỏi
          </span>
        </div>

        {/* Group Audio */}
        {group.urlAmThanh && (
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              🎧 Nghe đoạn hội thoại/bài nghe:
            </label>
            <audio controls className="w-full">
              <source src={group.urlAmThanh} type="audio/mpeg" />
              Trình duyệt không hỗ trợ audio.
            </audio>
          </div>
        )}

        {/* Group Image */}
        {group.urlHinhAnh && (
          <div className="mb-4">
            <img
              src={group.urlHinhAnh}
              alt="Group content"
              className="max-w-full h-auto rounded-lg shadow-md"
            />
          </div>
        )}

        {/* Reading Passage */}
        {group.noiDung && (
          <div className="mb-4 p-6 bg-white rounded-lg border-l-4 border-indigo-500 shadow-sm">
            <div
              className="prose max-w-none text-gray-800 leading-relaxed"
              dangerouslySetInnerHTML={{ __html: group.noiDung }}
            />
          </div>
        )}
      </div>

      {/* Questions in Group */}
      <div className="space-y-4">
        <h4 className="text-lg font-semibold text-gray-700 mb-3">
          Câu hỏi:
        </h4>
        {questions
          .sort((a, b) => a.thuTu - b.thuTu)
          .map((item, index) => (
            <QuestionCard
              key={item.cauHoi.id}
              question={item.cauHoi}
              questionNumber={startQuestionNumber + index}
              selectedAnswer={
                item.cauHoi.id ? selectedAnswers.get(item.cauHoi.id) : undefined
              }
              onAnswerSelect={onAnswerSelect}
              showResult={showResult}
              correctAnswer={
                item.cauHoi.id ? correctAnswers?.get(item.cauHoi.id) : undefined
              }
            />
          ))}
      </div>
    </div>
  );
}
