import { useState, useEffect, useCallback } from "react";

interface ExamTimerProps {
  totalMinutes: number;
  onTimeUp: () => void;
  isPaused?: boolean;
}

export function ExamTimer({
  totalMinutes,
  onTimeUp,
  isPaused = false,
}: ExamTimerProps) {
  const [secondsLeft, setSecondsLeft] = useState(() => {
    const saved = localStorage.getItem("exam-time-left");
    return saved ? parseInt(saved) : totalMinutes * 60;
  });
  const [totalSeconds] = useState(() => {
    const saved = localStorage.getItem("exam-total-time");
    return saved ? parseInt(saved) : totalMinutes * 60;
  });

  useEffect(() => {
    // Save total time on mount
    localStorage.setItem("exam-total-time", totalSeconds.toString());
  }, []);

  useEffect(() => {
    if (isPaused || secondsLeft <= 0) return;

    const timer = setInterval(() => {
      setSecondsLeft((prev) => {
        const newTime = prev - 1;
        localStorage.setItem("exam-time-left", newTime.toString());

        if (newTime <= 0) {
          clearInterval(timer);
          localStorage.removeItem("exam-time-left");
          onTimeUp();
          return 0;
        }

        return newTime;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [isPaused, secondsLeft, onTimeUp]);

  const minutes = Math.floor(secondsLeft / 60);
  const seconds = secondsLeft % 60;

  const getTimerColor = () => {
    const percentLeft = (secondsLeft / totalSeconds) * 100;
    if (percentLeft > 20) return "text-green-600 bg-green-50 border-green-200";
    if (percentLeft > 10)
      return "text-yellow-600 bg-yellow-50 border-yellow-200";
    return "text-red-600 bg-red-50 border-red-200";
  };

  const getWarningMessage = () => {
    if (secondsLeft <= 60) return "⚠️ Còn 1 phút!";
    if (secondsLeft <= 300) return "⚠️ Còn 5 phút!";
    return null;
  };

  const warningMessage = getWarningMessage();

  return (
    <div className={`sticky top-0 z-10 p-4 border-2 rounded-lg ${getTimerColor()}`}>
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <svg
            className="w-6 h-6"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <div>
            <div className="text-sm font-medium">Thời gian còn lại</div>
            <div className="text-2xl font-bold font-mono">
              {String(minutes).padStart(2, "0")}:
              {String(seconds).padStart(2, "0")}
            </div>
          </div>
        </div>

        {warningMessage && (
          <div className="animate-pulse font-semibold">{warningMessage}</div>
        )}

        {isPaused && (
          <div className="px-3 py-1 bg-gray-200 text-gray-700 rounded-full text-sm font-medium">
            Đã tạm dừng
          </div>
        )}
      </div>

      {/* Progress Bar */}
      <div className="mt-3 w-full bg-gray-200 rounded-full h-2">
        <div
          className="h-2 rounded-full transition-all duration-1000"
          style={{
            width: `${(secondsLeft / totalSeconds) * 100}%`,
            backgroundColor:
              secondsLeft <= 60
                ? "#ef4444"
                : secondsLeft <= 300
                ? "#f59e0b"
                : "#10b981",
          }}
        />
      </div>
    </div>
  );
}
