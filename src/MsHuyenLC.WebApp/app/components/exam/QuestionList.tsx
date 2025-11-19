import type { QuestionsGroupedResponse } from "~/types";
import { QuestionGroup } from "./QuestionGroup";
import { QuestionCard } from "./QuestionCard";

interface QuestionListProps {
  data: QuestionsGroupedResponse;
  selectedAnswers: Map<string, string>;
  onAnswerSelect: (questionId: string, answer: string) => void;
  showResult?: boolean;
  correctAnswers?: Map<string, string>;
}

export function QuestionList({
  data,
  selectedAnswers,
  onAnswerSelect,
  showResult = false,
  correctAnswers,
}: QuestionListProps) {
  let questionCounter = 0;

  // Group questions by nhomCauHoiId
  const groupedByNhom = new Map<
    string,
    Array<{ cauHoi: any; thuTu: number }>
  >();

  data.groupedQuestions.forEach((item) => {
    const groupId = item.nhomCauHoiId || "unknown";
    if (!groupedByNhom.has(groupId)) {
      groupedByNhom.set(groupId, []);
    }
    groupedByNhom.get(groupId)!.push({
      cauHoi: item.cauHoi,
      thuTu: item.thuTu,
    });
  });

  return (
    <div className="space-y-6">
      {/* Render Question Groups */}
      {Array.from(groupedByNhom.entries()).map(([groupId, questions]) => {
        const startNumber = questionCounter + 1;
        questionCounter += questions.length;

        // Find group info from first question
        const firstQuestion = questions[0]?.cauHoi;
        const groupInfo = firstQuestion?.cacNhom?.[0]?.nhom;

        return (
          <QuestionGroup
            key={groupId}
            group={groupInfo || { id: groupId }}
            questions={questions}
            startQuestionNumber={startNumber}
            selectedAnswers={selectedAnswers}
            onAnswerSelect={onAnswerSelect}
            showResult={showResult}
            correctAnswers={correctAnswers}
          />
        );
      })}

      {/* Render Standalone Questions */}
      {data.standaloneQuestions
        .sort((a, b) => a.thuTu - b.thuTu)
        .map((item) => {
          questionCounter++;
          return (
            <QuestionCard
              key={item.cauHoi.id}
              question={item.cauHoi}
              questionNumber={questionCounter}
              selectedAnswer={
                item.cauHoi.id
                  ? selectedAnswers.get(item.cauHoi.id)
                  : undefined
              }
              onAnswerSelect={onAnswerSelect}
              showResult={showResult}
              correctAnswer={
                item.cauHoi.id ? correctAnswers?.get(item.cauHoi.id) : undefined
              }
            />
          );
        })}

      {/* No Questions */}
      {data.totalQuestions === 0 && (
        <div className="text-center py-12">
          <svg
            className="mx-auto h-12 w-12 text-gray-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
            />
          </svg>
          <h3 className="mt-2 text-sm font-medium text-gray-900">
            Không có câu hỏi
          </h3>
          <p className="mt-1 text-sm text-gray-500">
            Đề thi này chưa có câu hỏi nào.
          </p>
        </div>
      )}
    </div>
  );
}
