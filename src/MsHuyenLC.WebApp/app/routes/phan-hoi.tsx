import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { Send, MessageSquare, Star } from "lucide-react";
import { getJwtToken } from "~/apis/Auth";
import { createPhanHoi } from "~/apis/PhanHoi";
import { getByTaiKhoanId } from "~/apis/HocVien";
import { getProfile } from "~/apis/Profile";

export default function FeedbackPage() {
  const navigate = useNavigate();
  const [rating, setRating] = useState(5);
  const [submitting, setSubmitting] = useState(false);
  const [message, setMessage] = useState("");
  const [hocVienId, setHocVienId] = useState<string>("");
  const [formData, setFormData] = useState({
    tieuDe: "",
    noiDung: "",
    loai: "general",
  });

  // Check if user is logged in and get student ID
  useEffect(() => {
    const jwtToken = getJwtToken();
    if (!jwtToken) {
      navigate("/dang-nhap");
      return;
    }

    // Get student ID from profile
    const loadStudentId = async () => {
      try {
        const profileRes = await getProfile();
        if (profileRes.success && profileRes.data?.id) {
          const studentRes = await getByTaiKhoanId(profileRes.data.id);
          if (studentRes.success && studentRes.data?.id) {
            setHocVienId(studentRes.data.id);
          }
        }
      } catch (error) {
        console.error("Error loading student ID:", error);
      }
    };

    loadStudentId();
  }, [navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setMessage("");

    if (!formData.tieuDe.trim() || !formData.noiDung.trim()) {
      setMessage("Vui lòng điền đầy đủ thông tin");
      setSubmitting(false);
      return;
    }

    if (!hocVienId) {
      setMessage("Không tìm thấy thông tin học viên");
      setSubmitting(false);
      return;
    }

    try {
      const result = await createPhanHoi({
        hocVienId: hocVienId,
        loaiPhanHoi: formData.loai,
        tieuDe: formData.tieuDe,
        noiDung: formData.noiDung,
      });

      if (result.success) {
        setMessage("Cảm ơn bạn đã gửi phản hồi! Chúng tôi sẽ xem xét và phản hồi sớm nhất.");
        setFormData({ tieuDe: "", noiDung: "", loai: "general" });
        setRating(5);

        setTimeout(() => {
          navigate("/");
        }, 2000);
      } else {
        setMessage(result.message || "Gửi phản hồi thất bại");
      }
    } catch (error) {
      setMessage("Có lỗi xảy ra khi gửi phản hồi");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-24 px-4 sm:px-6 lg:px-8">
      <div className="max-w-3xl mx-auto">
        {/* Header */}
        <div className="text-center mb-12">
          <div className="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center mx-auto mb-4">
            <MessageSquare className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-4xl font-bold text-gray-900 mb-2">Gửi phản hồi</h1>
          <p className="text-gray-600">
            Ý kiến của bạn giúp chúng tôi cải thiện chất lượng dịch vụ
          </p>
        </div>

        {/* Feedback Form */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Rating */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-3">
                Đánh giá tổng quan
              </label>
              <div className="flex items-center gap-2">
                {[1, 2, 3, 4, 5].map((star) => (
                  <button
                    key={star}
                    type="button"
                    onClick={() => setRating(star)}
                    className="transition-transform hover:scale-110"
                  >
                    <Star
                      className={`w-10 h-10 ${
                        star <= rating
                          ? "fill-yellow-400 text-yellow-400"
                          : "text-gray-300"
                      }`}
                    />
                  </button>
                ))}
                <span className="ml-4 text-lg font-semibold text-gray-900">
                  {rating}/5
                </span>
              </div>
            </div>

            {/* Feedback Type */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Loại phản hồi <span className="text-red-500">*</span>
              </label>
              <select
                value={formData.loai}
                onChange={(e) => setFormData({ ...formData, loai: e.target.value })}
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                required
              >
                <option value="general">Góp ý chung</option>
                <option value="course">Về khóa học</option>
                <option value="teacher">Về giáo viên</option>
                <option value="facility">Về cơ sở vật chất</option>
                <option value="service">Về dịch vụ</option>
                <option value="complaint">Khiếu nại</option>
                <option value="other">Khác</option>
              </select>
            </div>

            {/* Title */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Tiêu đề <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                value={formData.tieuDe}
                onChange={(e) => setFormData({ ...formData, tieuDe: e.target.value })}
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Nhập tiêu đề phản hồi..."
                required
              />
            </div>

            {/* Content */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Nội dung <span className="text-red-500">*</span>
              </label>
              <textarea
                value={formData.noiDung}
                onChange={(e) => setFormData({ ...formData, noiDung: e.target.value })}
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none"
                rows={8}
                placeholder="Vui lòng chia sẻ chi tiết ý kiến của bạn..."
                required
              />
              <p className="text-sm text-gray-500 mt-2">
                Tối thiểu 20 ký tự
              </p>
            </div>

            {/* Message */}
            {message && (
              <div className="bg-green-50 border border-green-200 text-green-800 px-4 py-3 rounded-lg">
                <p className="text-sm">{message}</p>
              </div>
            )}

            {/* Submit Button */}
            <div className="flex gap-4 pt-4">
              <button
                type="submit"
                disabled={submitting || formData.noiDung.length < 20}
                className="flex-1 bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 font-semibold"
              >
                {submitting ? (
                  <>
                    <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                    Đang gửi...
                  </>
                ) : (
                  <>
                    <Send className="w-5 h-5" />
                    Gửi phản hồi
                  </>
                )}
              </button>
              <button
                type="button"
                onClick={() => navigate(-1)}
                className="px-6 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-semibold"
              >
                Hủy
              </button>
            </div>
          </form>
        </div>

        {/* Contact Info */}
        <div className="mt-8 grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="bg-white rounded-lg p-4 border border-gray-200 text-center">
            <p className="text-sm text-gray-600 mb-1">Hotline</p>
            <p className="font-semibold text-gray-900">1900-xxxx</p>
          </div>
          <div className="bg-white rounded-lg p-4 border border-gray-200 text-center">
            <p className="text-sm text-gray-600 mb-1">Email</p>
            <p className="font-semibold text-gray-900">support@hplc.edu.vn</p>
          </div>
          <div className="bg-white rounded-lg p-4 border border-gray-200 text-center">
            <p className="text-sm text-gray-600 mb-1">Thời gian làm việc</p>
            <p className="font-semibold text-gray-900">8:00 - 17:00</p>
          </div>
        </div>
      </div>
    </div>
  );
}
