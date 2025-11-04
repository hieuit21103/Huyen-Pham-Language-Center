import { Search, CheckCircle, XCircle, Clock, Eye, AlertCircle, Trash2 } from "lucide-react";
import { useState, useEffect } from "react";
import { getDangKyKhachs, updateDangKyKhach, deleteDangKyKhach } from "~/apis/DangKy";
import { getKhoaHocs } from "~/apis/KhoaHoc";
import { TrangThaiDangKy } from "~/types/enums";
import { setLightTheme } from "./_layout";
import Pagination from "~/components/Pagination";
import type { DangKyKhach } from "~/types/course.types";
import type { KhoaHoc } from "~/types/course.types";


export default function AdminRegistrations() {
    const [searchTerm, setSearchTerm] = useState("");
    const [registrations, setRegistrations] = useState<DangKyKhach[]>([]);
    const [courses, setCourses] = useState<KhoaHoc[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedCourse, setSelectedCourse] = useState("");
    const [selectedStatus, setSelectedStatus] = useState("");
    const [message, setMessage] = useState("");

    // Pagination
    const [currentPage, setCurrentPage] = useState(1);
    const pageSize = 10;

    useEffect(() => {
        setLightTheme();
        loadData();
    }, []);

    useEffect(() => {
        setCurrentPage(1);
    }, [searchTerm, selectedCourse, selectedStatus]);

    const loadData = async () => {
        setLoading(true);
        const [registrationsRes, coursesRes] = await Promise.all([
            getDangKyKhachs({ pageNumber: 1, pageSize: 1000, sortBy: 'ngayDangKy', sortOrder: 'desc' }),
            getKhoaHocs({ pageNumber: 1, pageSize: 1000 })
        ]);

        if (registrationsRes.success && Array.isArray(registrationsRes.data)) {
            setRegistrations(registrationsRes.data);
        }

        if (coursesRes.success && Array.isArray(coursesRes.data)) {
            setCourses(coursesRes.data);
        }

        setLoading(false);
    };

    const handleApprove = async (id: string) => {
        if (!confirm("Bạn có chắc chắn muốn duyệt đăng ký này?")) return;

        const response = await updateDangKyKhach(id, { trangThai: TrangThaiDangKy.DaDuyet });
        setMessage(response.message || "");
        if (response.success) {
            loadData();
            setTimeout(() => setMessage(""), 3000);
        }
    };

    const handleReject = async (id: string) => {
        if (!confirm("Bạn có chắc chắn muốn từ chối đăng ký này?")) return;

        const response = await updateDangKyKhach(id, { trangThai: TrangThaiDangKy.Huy });
        setMessage(response.message || "");
        if (response.success) {
            loadData();
            setTimeout(() => setMessage(""), 3000);
        }
    };

    const handleDelete = async (id: string) => {
        if (!confirm("Bạn có chắc chắn muốn xóa đăng ký này? Hành động này không thể hoàn tác.")) return;

        const response = await deleteDangKyKhach(id);
        setMessage(response.message || "");
        if (response.success) {
            loadData();
            setTimeout(() => setMessage(""), 3000);
        }
    };

    const handleViewDetail = (id: string) => {
        // Hiện tại chưa có trang chi tiết, chỉ là placeholder
        alert(`Xem chi tiết đăng ký với ID: ${id}`);
    }

    const getStatusBadge = (status?: TrangThaiDangKy) => {
        switch (status) {
            case TrangThaiDangKy.DaDuyet:
                return (
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">
                        <CheckCircle className="w-3 h-3 mr-1" />
                        Đã duyệt
                    </span>
                );
            case TrangThaiDangKy.DaXepLop:
                return (
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800">
                        <CheckCircle className="w-3 h-3 mr-1" />
                        Đã xếp lớp
                    </span>
                );
            case TrangThaiDangKy.DaThanhToan:
                return (
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-purple-100 text-purple-800">
                        <Clock className="w-3 h-3 mr-1" />
                        Chờ xếp lớp
                    </span>
                );
            case TrangThaiDangKy.Huy:
                return (
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-gray-100 text-gray-800">
                        <XCircle className="w-3 h-3 mr-1" />
                        Đã hủy
                    </span>
                );
            default:
                return (
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-yellow-100 text-yellow-800">
                        <Clock className="w-3 h-3 mr-1" />
                        Chờ xử lý
                    </span>
                );
        }
    };

    const filteredRegistrations = registrations.filter(reg => {
        const matchSearch =
            reg.hoTen?.toLowerCase().includes(searchTerm.toLowerCase()) ||
            reg.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
            reg.soDienThoai?.includes(searchTerm);

        const matchCourse = !selectedCourse || reg.khoaHocId === selectedCourse;
        const matchStatus = !selectedStatus || reg.trangThai?.toString() === selectedStatus;

        return matchSearch && matchCourse && matchStatus;
    });

    const getPaginatedData = () => {
        const startIndex = (currentPage - 1) * pageSize;
        const endIndex = startIndex + pageSize;
        return filteredRegistrations.slice(startIndex, endIndex);
    };

    const handlePageChange = (page: number) => {
        setCurrentPage(page);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const getStatusCount = (status: TrangThaiDangKy) => {
        return filteredRegistrations.filter(r => r.trangThai === status).length;
    };

    return (
        <div className="space-y-6">
            {message && (
                <div className={`${message.includes("thất bại") || message.includes("Lỗi") ? "bg-red-100 border-red-400 text-red-700" : "bg-green-100 border-green-400 text-green-700"} border px-4 py-3 rounded-lg`}>
                    {message}
                </div>
            )}

            {/* Header */}
            <div>
                <h1 className="text-3xl font-bold text-gray-900">Quản lý đăng ký</h1>
                <p className="text-gray-600 mt-1">Danh sách đăng ký khóa học</p>
            </div>

            {/* Stats */}
            {loading ? (
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    {[1, 2, 3].map((i) => (
                        <div key={i} className="bg-gray-50 border border-gray-200 rounded-xl p-6">
                            <div className="h-20 bg-gray-200 animate-pulse rounded"></div>
                        </div>
                    ))}
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div className="bg-yellow-50 border border-yellow-200 rounded-xl p-6">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-yellow-700 text-sm font-medium mb-1">Chờ xử lý</p>
                                <p className="text-3xl font-bold text-yellow-900">{getStatusCount(TrangThaiDangKy.ChoDuyet)}</p>
                            </div>
                            <div className="w-12 h-12 bg-yellow-100 rounded-lg flex items-center justify-center">
                                <Clock className="w-6 h-6 text-yellow-700" />
                            </div>
                        </div>
                    </div>
                    <div className="bg-green-50 border border-green-200 rounded-xl p-6">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-green-700 text-sm font-medium mb-1">Đã duyệt</p>
                                <p className="text-3xl font-bold text-green-900">{getStatusCount(TrangThaiDangKy.DaDuyet)}</p>
                            </div>
                            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                                <CheckCircle className="w-6 h-6 text-green-700" />
                            </div>
                        </div>
                    </div>
                    <div className="bg-red-50 border border-red-200 rounded-xl p-6">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-red-700 text-sm font-medium mb-1">Từ chối</p>
                                <p className="text-3xl font-bold text-red-900">{getStatusCount(TrangThaiDangKy.Huy)}</p>
                            </div>
                            <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
                                <XCircle className="w-6 h-6 text-red-700" />
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Filters */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                        <input
                            type="text"
                            placeholder="Tìm kiếm theo tên, email, SĐT..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                        />
                    </div>
                    <select
                        value={selectedCourse}
                        onChange={(e) => setSelectedCourse(e.target.value)}
                        className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                        <option value="">Tất cả khóa học</option>
                        {courses.map((course) => (
                            <option key={course.id} value={course.id}>
                                {course.tenKhoaHoc}
                            </option>
                        ))}
                    </select>
                    <select
                        value={selectedStatus}
                        onChange={(e) => setSelectedStatus(e.target.value)}
                        className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                        <option value="">Tất cả trạng thái</option>
                        <option value={TrangThaiDangKy.ChoDuyet.toString()}>Chờ duyệt</option>
                        <option value={TrangThaiDangKy.DaDuyet.toString()}>Đã duyệt</option>
                        <option value={TrangThaiDangKy.Huy.toString()}>Từ chối</option>
                        <option value={TrangThaiDangKy.DaThanhToan.toString()}>Đã thanh toán</option>
                        <option value={TrangThaiDangKy.DaXepLop.toString()}>Đã xếp lớp</option>
                        <option value={TrangThaiDangKy.Huy.toString()}>Đã hủy</option>
                    </select>
                </div>
            </div>

            {/* Registrations Table */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200">
                {loading ? (
                    <div className="p-8 text-center">
                        <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
                        <p className="text-gray-600">Đang tải dữ liệu...</p>
                    </div>
                ) : filteredRegistrations.length === 0 ? (
                    <div className="p-8 text-center">
                        <AlertCircle className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                        <p className="text-gray-600 text-lg">Không tìm thấy đăng ký nào</p>
                    </div>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead className="bg-gray-50 border-b border-gray-200">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Học viên
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Liên hệ
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Khóa học
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Ngày đăng ký
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Trạng thái
                                    </th>
                                    <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Thao tác
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-gray-200">
                                {getPaginatedData().map((registration) => (
                                    <tr key={registration.id} className="hover:bg-gray-50 transition-colors">
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <div className="flex items-center">
                                                <div className="w-10 h-10 bg-gray-900 rounded-full flex items-center justify-center text-white font-semibold">
                                                    {registration.hoTen?.charAt(0) || "?"}
                                                </div>
                                                <div className="ml-3">
                                                    <p className="text-sm font-medium text-gray-900">
                                                        {registration.hoTen || "Chưa có tên"}
                                                    </p>
                                                    {registration.ghiChu && (
                                                        <p className="text-xs text-gray-500 italic">{registration.ghiChu}</p>
                                                    )}
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <p className="text-sm text-gray-900">{registration.email || "—"}</p>
                                            <p className="text-sm text-gray-600">{registration.soDienThoai || "—"}</p>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">
                                            {courses.find(c => c.id === registration.khoaHocId)?.tenKhoaHoc || "—"}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                                            {registration.ngayDangKy
                                                ? new Date(registration.ngayDangKy).toLocaleDateString('vi-VN')
                                                : "—"}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            {getStatusBadge(registration.trangThai)}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                            <div className="flex items-center justify-end space-x-2">
                                                {registration.trangThai === TrangThaiDangKy.ChoDuyet && (
                                                    <>
                                                        <button
                                                            onClick={() => handleApprove(registration.id!)}
                                                            className="text-green-600 hover:text-green-900 px-3 py-1 hover:bg-green-50 rounded-lg transition-colors font-semibold"
                                                        >
                                                            Duyệt
                                                        </button>
                                                        <button
                                                            onClick={() => handleReject(registration.id!)}
                                                            className="text-red-600 hover:text-red-900 px-3 py-1 hover:bg-red-50 rounded-lg transition-colors font-semibold"
                                                        >
                                                            Từ chối
                                                        </button>
                                                    </>
                                                )}
                                                <button className="text-blue-600 hover:text-blue-900 p-2 hover:bg-blue-50 rounded-lg transition-colors">
                                                    <Eye className="w-4 h-4" />
                                                </button>
                                                <button
                                                    onClick={() => handleDelete(registration.id!)}
                                                    className="text-red-600 hover:text-red-900 p-2 hover:bg-red-50 rounded-lg transition-colors"
                                                    title="Xóa đăng ký"
                                                >
                                                    <Trash2 className="w-4 h-4" />
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}

                {/* Pagination */}
                {!loading && filteredRegistrations.length > pageSize && (
                    <div className="flex justify-center p-6 border-t border-gray-200">
                        <Pagination
                            currentPage={currentPage}
                            totalCount={filteredRegistrations.length}
                            pageSize={pageSize}
                            onPageChange={handlePageChange}
                        />
                    </div>
                )}
            </div>
        </div>
    );
}
