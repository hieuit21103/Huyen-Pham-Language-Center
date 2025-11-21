import { useState, useEffect } from "react";
import { getProfile } from "~/apis/Profile";
import { registerStudent, getDangKysByHocVien } from "~/apis/DangKy";
import type { DangKyRequest } from "~/types/index";
import { getByTaiKhoanId } from "~/apis/HocVien";
import { getKhoaHocs } from "~/apis/KhoaHoc";
import { Asset } from "~/assets/Asset";

interface DangKy {
    id?: string;
    khoaHocId?: string;
    hocVienId?: string;
}

export default function KhoaHocPage() {
    const [loading, setLoading] = useState(true);
    const [courses, setCourses] = useState<any[]>([]);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [studentId, setStudentId] = useState<string | undefined>("");
    const [registeredCourseIds, setRegisteredCourseIds] = useState<string[]>([]);
    const [message, setMessage] = useState("");

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        setLoading(true);

        const profileRes = await getProfile();
        if (profileRes.success && profileRes.data) {
            setIsLoggedIn(true);

            const hocVienRes = await getByTaiKhoanId(profileRes.data.id);
            if (hocVienRes.success && hocVienRes.data) {
                setStudentId(hocVienRes.data.id);

                const dangKyRes = await getDangKysByHocVien(hocVienRes.data.id);
                if (dangKyRes.success && dangKyRes.data) {
                    const courseIds = dangKyRes.data.map((dk: any) => dk.khoaHoc.id!);
                    setRegisteredCourseIds(courseIds);
                }
            }
        }

        try {
            const response = await getKhoaHocs();
            if (response.success && response.data) {
                const formattedCourses = response.data.filter((course: any) => !registeredCourseIds.includes(course.id)).map((course: any) => ({
                    id: course.id,
                    title: course.tenKhoaHoc || 'Chưa có tên',
                    description: course.moTa || 'Chưa có mô tả',
                    level: 'Tất cả trình độ',
                    price: course.hocPhi ? `${course.hocPhi.toLocaleString('vi-VN')}đ` : 'Liên hệ',
                    duration: course.thoiLuong ? `${course.thoiLuong} buổi` : 'Liên hệ',
                    students: course.lopHocs?.length || 0
                }));
                setCourses(formattedCourses);
            }
        } catch (error) {
            console.error('Lỗi khi tải khóa học:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleCourseRegister = async (courseId: string, courseName: string) => {
        if (!isLoggedIn) {
            window.location.href = '/#register';
            return;
        }

        if (!studentId) {
            setMessage("Lỗi: Không tìm thấy thông tin học viên. Vui lòng đăng nhập lại!");
            setTimeout(() => setMessage(""), 5000);
            return;
        }

        const request: DangKyRequest = {
            hocVienId: studentId,
            khoaHocId: courseId
        };

        const response = await registerStudent(request);

        if (response.success) {
            setMessage(response.message || `Đăng ký khóa học "${courseName}" thành công!`);
            setRegisteredCourseIds(prev => [...prev, courseId]);
            setTimeout(() => setMessage(""), 3000);
        } else {
            setMessage(response.message || "Lỗi: Đăng ký thất bại. Vui lòng thử lại!");
            setTimeout(() => setMessage(""), 5000);
        }
    };

    return (
        <div>
            {message && (
                <div 
                    className={`fixed top-24 right-6 z-50 min-w-80 max-w-md ${message.includes("thất bại") || message.includes("Lỗi") ? "bg-red-100 border-red-400 text-red-700" : "bg-green-100 border-green-400 text-green-700"} border-l-4 px-6 py-4 rounded-lg shadow-xl transition-all duration-500 ease-in-out`}
                    style={{
                        animation: 'slideInRight 0.5s ease-out'
                    }}
                >
                    <div className="flex items-start gap-3">
                        <p className="font-semibold flex-1">{message}</p>
                    </div>
                </div>
            )}

            <style>{`
                @keyframes slideInRight {
                    from {
                        transform: translateX(100%);
                        opacity: 0;
                    }
                    to {
                        transform: translateX(0);
                        opacity: 1;
                    }
                }
            `}</style>

            <h2 className="text-4xl font-bold text-center text-gray-900 mb-4">
                Các Khóa Học Nổi Bật
            </h2>
            <p className="text-center text-gray-600 mb-12 text-lg">
                Chương trình học được thiết kế phù hợp với mọi trình độ và mục tiêu
            </p>

            {/* Course Showcase Banner */}
            <div className="bg-gray-100 rounded-3xl p-8 mb-12 shadow-2xl border-2 border-gray-200">
                <div className="grid md:grid-cols-2 gap-8 items-center">
                    <div className="aspect-video bg-gray-200 rounded-2xl flex items-center justify-center border-4 border-gray-300">
                        <img src={Asset.classroomImage} alt="Classroom" className="object-contain rounded-2xl shadow-lg" />
                    </div>
                    <div className="text-gray-900">
                        <h3 className="text-3xl font-bold mb-4">
                            Học Với Đội Ngũ Chuyên Gia
                        </h3>
                        <p className="text-gray-700 text-lg mb-6">
                            100% giáo viên có chứng chỉ quốc tế, kinh nghiệm giảng dạy trên 5 năm
                        </p>
                        <div className="space-y-3 text-gray-700">
                            <div className="flex items-center">
                                <div className="w-2 h-2 bg-gray-900 rounded-full mr-3"></div>
                                <span>Phương pháp giảng dạy tương tác hiện đại</span>
                            </div>
                            <div className="flex items-center">
                                <div className="w-2 h-2 bg-gray-900 rounded-full mr-3"></div>
                                <span>Lớp học nhỏ tối đa 15 học viên</span>
                            </div>
                            <div className="flex items-center">
                                <div className="w-2 h-2 bg-gray-900 rounded-full mr-3"></div>
                                <span>Hỗ trợ học tập 24/7 qua app</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {loading ? (
                <div className="text-center py-12">
                    <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
                    <p className="mt-4 text-gray-900 font-semibold">Đang tải khóa học...</p>
                </div>
            ) : courses.length === 0 ? (
                <div className="text-center py-12 bg-white rounded-2xl shadow-lg">
                    <p className="text-gray-600 text-lg">Chưa có khóa học nào</p>
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                    {courses
                        .filter(course => !registeredCourseIds.includes(course.id))
                        .map((course, index) => (
                            <div
                                key={course.id || index}
                                className="bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl hover:scale-105 transition-all border-2 border-gray-100 flex flex-col"
                            >
                                <h3 className="text-2xl font-bold text-gray-900 mb-3 text-center min-h-[64px] flex items-center justify-center">
                                    {course.title}
                                </h3>
                                <p className="text-gray-600 mb-4 text-center line-clamp-3 min-h-[72px]">{course.description}</p>

                                <div className="text-center mb-4">
                                    <span className="inline-block bg-gray-100 text-gray-900 px-3 py-1 rounded-full text-sm font-semibold">
                                        {course.level}
                                    </span>
                                </div>

                                <div className="border-t border-gray-100 pt-4 mb-4">
                                    <div className="text-3xl font-bold text-gray-900 text-center">
                                        {course.price}
                                    </div>
                                </div>

                                <div className="space-y-2 mb-6 flex-grow">
                                    <p className="text-gray-600 text-center">
                                        <strong>Thời lượng:</strong> {course.duration}
                                    </p>
                                    {course.students > 0 && (
                                        <p className="text-gray-500 text-center text-sm">
                                            👥 {course.students} lớp đang mở
                                        </p>
                                    )}
                                </div>

                                <button
                                    onClick={() => handleCourseRegister(course.id, course.title)}
                                    className="w-full bg-gray-900 text-white py-3 rounded-lg font-bold hover:bg-gray-800 transition-all mt-auto"
                                >
                                    Đăng Ký
                                </button>
                            </div>
                        ))}
                </div>
            )}
        </div>
    );
}
