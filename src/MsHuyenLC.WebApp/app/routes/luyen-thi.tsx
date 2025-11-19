import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { getProfile } from "~/apis/Profile";
import { getDeThis } from "~/apis/DeThi";
import { BookOpen, Clock, FileText, Play } from "lucide-react";
import type { DeThi } from "~/types/index";

export default function LuyenThi() {
    const navigate = useNavigate();
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [loading, setLoading] = useState(true);
    const [deThis, setDeThis] = useState<DeThi[]>([]);
    const [searchTerm, setSearchTerm] = useState("");

    useEffect(() => {
        checkLoginAndLoadData();
    }, []);

    const checkLoginAndLoadData = async () => {
        setLoading(true);
        const profileRes = await getProfile();

        if (profileRes.success && profileRes.data) {
            setIsLoggedIn(true);

            const deThiRes = await getDeThis();
            if (deThiRes.success && Array.isArray(deThiRes.data)) {
                const deThiLuyenTap = deThiRes.data.filter((de: DeThi) => !de.kyThiId);
                setDeThis(deThiLuyenTap);
            }
        } else {
            setIsLoggedIn(false);
        }

        setLoading(false);
    };

    const handleCourseRegister = () => {
        window.location.href = '/#register';
    };

    const handleStartExam = (deThiId: string) => {
        navigate(`/thi?deThiId=${deThiId}`);
    };

    const filteredDeThis = deThis.filter(de => {
        const matchSearch = de.tenDe?.toLowerCase().includes(searchTerm.toLowerCase());
        return matchSearch;
    });

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <div className="text-center">
                    <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
                    <p className="text-gray-600">Đang tải...</p>
                </div>
            </div>
        );
    }

    if (!isLoggedIn) {
        return (
            <div>
                <h2 className="text-4xl font-bold text-center text-gray-900 mb-12">
                    Luyện Đề Online
                </h2>
                <div className="max-w-4xl mx-auto bg-white rounded-2xl shadow-2xl p-8 border-2 border-gray-100">
                    <div className="text-center mb-8">
                        <h3 className="text-3xl font-bold text-gray-900 mb-6">
                            Ngân Hàng Đề Thi
                        </h3>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                        <div className="text-center p-6 bg-gray-50 rounded-xl border-2 border-gray-100">
                            <div className="text-5xl mb-4"></div>
                            <h4 className="text-xl font-bold text-gray-900 mb-2">
                                500+ Đề Thi IELTS
                            </h4>
                            <p className="text-gray-600">Cập nhật liên tục từ IDP & BC</p>
                        </div>
                        <div className="text-center p-6 bg-gray-50 rounded-xl border-2 border-gray-100">
                            <div className="text-5xl mb-4"></div>
                            <h4 className="text-xl font-bold text-gray-900 mb-2">
                                Chấm Bài Tự Động
                            </h4>
                        </div>
                        <div className="text-center p-6 bg-gray-50 rounded-xl border-2 border-gray-100">
                            <div className="text-5xl mb-4"></div>
                            <h4 className="text-xl font-bold text-gray-900 mb-2">
                                Theo Dõi Tiến Độ
                            </h4>
                            <p className="text-gray-600">Báo cáo chi tiết năng lực</p>
                        </div>
                    </div>

                    <div className="text-center mb-6">
                        <p className="text-xl font-bold text-gray-900">
                            Miễn phí cho học viên đăng ký khóa học!
                        </p>
                    </div>

                    <button
                        onClick={handleCourseRegister}
                        className="w-full bg-gray-900 text-white py-4 rounded-lg font-bold text-lg hover:bg-gray-800 hover:scale-105 transition-all"
                    >
                        Truy Cập Ngay
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-4xl font-bold text-gray-900 mb-2">Luyện Đề Online</h1>
                <p className="text-gray-600">Hệ thống đề thi luyện tập</p>
            </div>

            {/* Stats */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-6">
                    <div className="flex items-center space-x-3">
                        <BookOpen className="w-10 h-10 text-blue-600" />
                        <div>
                            <p className="text-blue-700 text-sm font-medium">Tổng số đề</p>
                            <p className="text-3xl font-bold text-blue-900">{deThis.length}</p>
                        </div>
                    </div>
                </div>
                <div className="bg-green-50 border border-green-200 rounded-lg p-6">
                    <div className="flex items-center space-x-3">
                        <FileText className="w-10 h-10 text-green-600" />
                        <div>
                            <p className="text-green-700 text-sm font-medium">Đề luyện tập</p>
                            <p className="text-3xl font-bold text-green-900">{deThis.length}</p>
                        </div>
                    </div>
                </div>
                <div className="bg-purple-50 border border-purple-200 rounded-lg p-6">
                    <div className="flex items-center space-x-3">
                        <Clock className="w-10 h-10 text-purple-600" />
                        <div>
                            <p className="text-purple-700 text-sm font-medium">Đã hoàn thành</p>
                            <p className="text-3xl font-bold text-purple-900">0</p>
                        </div>
                    </div>
                </div>
                <div className="bg-orange-50 border border-orange-200 rounded-lg p-6">
                    <div className="flex items-center space-x-3">
                        <Play className="w-10 h-10 text-orange-600" />
                        <div>
                            <p className="text-orange-700 text-sm font-medium">Điểm TB</p>
                            <p className="text-3xl font-bold text-orange-900">--</p>
                        </div>
                    </div>
                </div>
            </div>

            {/* Filters */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <input
                    type="text"
                    placeholder="Tìm kiếm đề thi..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                />
            </div>

            {/* Exam List */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filteredDeThis.length === 0 ? (
                    <div className="col-span-full text-center py-12">
                        <BookOpen className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                        <p className="text-gray-600 text-lg">Không tìm thấy đề thi nào</p>
                    </div>
                ) : (
                    filteredDeThis.map((deThi) => (
                        <div key={deThi.id} className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-lg transition-shadow">
                            <div className="mb-4">
                                <h3 className="text-xl font-bold text-gray-900">{deThi.tenDe || "Đề thi"}</h3>
                            </div>

                            <div className="space-y-2 mb-6">
                                <div className="flex items-center text-gray-600">
                                    <FileText className="w-4 h-4 mr-2" />
                                    <span className="text-sm">{deThi.tongCauHoi || 0} câu hỏi</span>
                                </div>
                                <div className="flex items-center text-gray-600">
                                    <Clock className="w-4 h-4 mr-2" />
                                    <span className="text-sm">{deThi.thoiLuongPhut || 0} phút</span>
                                </div>
                            </div>
                            <div className="mt-auto">
                                <button
                                    onClick={() => handleStartExam(deThi.id!)}
                                    className="w-full bg-gray-900 text-white py-3 rounded-lg font-semibold hover:bg-gray-800 transition-colors flex items-center justify-center space-x-2"
                                >
                                    <Play className="w-5 h-5" />
                                    <span>Bắt đầu làm bài</span>
                                </button>
                            </div>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}