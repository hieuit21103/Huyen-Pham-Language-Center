import { Search, Plus, Edit, Trash2, X, UserCheck } from "lucide-react";
import { useState, useEffect } from "react";
import { getPhanCongs, createPhanCong, deletePhanCong } from "~/apis/PhanCong";
import { getGiaoViens } from "~/apis/GiaoVien";
import { getLopHocs } from "~/apis/LopHoc";
import type { PhanCong, GiaoVien, LopHoc } from "~/types/index";
import { setLightTheme } from "./_layout";
import Pagination from "~/components/Pagination";

export default function AdminPhanCong() {
    const [searchTerm, setSearchTerm] = useState("");
    const [phanCongs, setPhanCongs] = useState<PhanCong[]>([]);
    const [giaoViens, setGiaoViens] = useState<GiaoVien[]>([]);
    const [lopHocs, setLopHocs] = useState<LopHoc[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [message, setMessage] = useState("");

    // Pagination
    const [currentPage, setCurrentPage] = useState(1);
    const pageSize = 10;

    const [formData, setFormData] = useState({
        giaoVienId: "",
        lopHocId: "",
    });

    useEffect(() => {
        setLightTheme();
        loadData();
    }, []);

    useEffect(() => {
        setCurrentPage(1);
    }, [searchTerm]);

    const loadData = async () => {
        setLoading(true);

        // Load phân công
        const pcResponse = await getPhanCongs();
        if (pcResponse.success && pcResponse.data) {
            setPhanCongs(pcResponse.data);
        }

        // Load giáo viên
        const gvResponse = await getGiaoViens();
        if (gvResponse.success && gvResponse.data) {
            setGiaoViens(gvResponse.data);
        }

        // Load lớp học
        const lhResponse = await getLopHocs();
        if (lhResponse.success && lhResponse.data) {
            setLopHocs(lhResponse.data);
        }

        setLoading(false);
    };

    const handleCreate = () => {
        setFormData({
            giaoVienId: "",
            lopHocId: "",
        });
        setShowModal(true);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const response = await createPhanCong(formData);
        setMessage(response.message || "");
        if (response.success) {
            loadData();
            setShowModal(false);
        }
    };

    const handleDelete = async (id: string) => {
        if (confirm("Bạn có chắc chắn muốn xóa phân công này?")) {
            const response = await deletePhanCong(id);
            setMessage(response.message || "");
            if (response.success) {
                loadData();
            }
        }
    };

    const filteredPhanCongs = phanCongs.filter(pc => {
        const giaoVienName = pc.giaoVien?.hoTen?.toLowerCase() || "";
        const lopHocName = pc.lopHoc?.tenLop?.toLowerCase() || "";
        const search = searchTerm.toLowerCase();
        return giaoVienName.includes(search) || lopHocName.includes(search);
    });

    const getPaginatedData = () => {
        const startIndex = (currentPage - 1) * pageSize;
        const endIndex = startIndex + pageSize;
        return filteredPhanCongs.slice(startIndex, endIndex);
    };

    const handlePageChange = (page: number) => {
        setCurrentPage(page);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    return (
        <div className="space-y-6">
            {message && (
                <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg">
                    {message}
                </div>
            )}

            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                <div>
                    <h1 className="text-3xl font-bold text-gray-900">Quản lý phân công giảng dạy</h1>
                    <p className="text-gray-600 mt-1">Phân công giáo viên cho các lớp học</p>
                </div>
                <button
                    onClick={handleCreate}
                    className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
                >
                    <Plus className="w-5 h-5" />
                    <span>Thêm phân công</span>
                </button>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <div className="relative">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                    <input
                        type="text"
                        placeholder="Tìm kiếm theo tên giáo viên hoặc lớp học..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    />
                </div>
            </div>

            {loading && (
                <div className="text-center py-12">
                    <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
                </div>
            )}

            {!loading && (
                <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead className="bg-gray-50 border-b border-gray-200">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        STT
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Giáo viên
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Lớp học
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Khóa học
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Email
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Số điện thoại
                                    </th>
                                    <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        Thao tác
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-gray-200">
                                {getPaginatedData().length === 0 ? (
                                    <tr>
                                        <td colSpan={7} className="px-6 py-12 text-center">
                                            <UserCheck className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                                            <p className="text-gray-600">Chưa có phân công nào</p>
                                        </td>
                                    </tr>
                                ) : (
                                    getPaginatedData().map((pc, index) => (
                                        <tr key={pc.id || index} className="hover:bg-gray-50 transition-colors">
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                                {index + 1}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="flex items-center">
                                                    <div className="w-10 h-10 bg-gray-900 rounded-full flex items-center justify-center text-white font-semibold">
                                                        {pc.giaoVien?.hoTen?.charAt(0) || "?"}
                                                    </div>
                                                    <div className="ml-3">
                                                        <p className="text-sm font-medium text-gray-900">
                                                            {pc.giaoVien?.hoTen || "—"}
                                                        </p>
                                                        <p className="text-xs text-gray-500">
                                                            {pc.giaoVien?.chuyenMon || "—"}
                                                        </p>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <p className="text-sm font-medium text-gray-900">
                                                    {pc.lopHoc?.tenLop || "—"}
                                                </p>
                                                <p className="text-xs text-gray-500">
                                                    Sĩ số: {pc.lopHoc?.siSoHienTai || 0}/{pc.lopHoc?.siSoToiDa || 0}
                                                </p>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                                {pc.lopHoc?.khoaHoc?.tenKhoaHoc || "—"}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                                {pc.giaoVien?.taiKhoan?.email || "—"}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                                {pc.giaoVien?.taiKhoan?.sdt || "—"}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-center">
                                                <button
                                                    onClick={() => handleDelete(pc.id!)}
                                                    className="text-red-600 hover:text-red-900 transition-colors"
                                                    title="Xóa"
                                                >
                                                    <Trash2 className="w-5 h-5" />
                                                </button>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>

                    {/* Pagination */}
                    {filteredPhanCongs.length > pageSize && (
                        <div className="flex justify-center mt-6">
                            <Pagination
                                currentPage={currentPage}
                                totalCount={filteredPhanCongs.length}
                                pageSize={pageSize}
                                onPageChange={handlePageChange}
                            />
                        </div>
                    )}
                </div>
            )}

            {showModal && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                    <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
                        <div className="p-6">
                            <div className="flex justify-between items-center mb-6">
                                <h2 className="text-2xl font-bold text-gray-900">Thêm phân công mới</h2>
                                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                                    <X className="w-6 h-6" />
                                </button>
                            </div>

                            <form onSubmit={handleSubmit} className="space-y-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Giáo viên <span className="text-red-500">*</span>
                                    </label>
                                    <select
                                        value={formData.giaoVienId}
                                        onChange={(e) => setFormData({ ...formData, giaoVienId: e.target.value })}
                                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                                        required
                                    >
                                        <option value="">-- Chọn giáo viên --</option>
                                        {giaoViens.map((gv) => (
                                            <option key={gv.id} value={gv.id}>
                                                {gv.hoTen} {gv.chuyenMon && `- ${gv.chuyenMon}`}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Lớp học <span className="text-red-500">*</span>
                                    </label>
                                    <select
                                        value={formData.lopHocId}
                                        onChange={(e) => setFormData({ ...formData, lopHocId: e.target.value })}
                                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                                        required
                                    >
                                        <option value="">-- Chọn lớp học --</option>
                                        {lopHocs.map((lh) => (
                                            <option key={lh.id} value={lh.id}>
                                                {lh.tenLop} - {lh.khoaHoc?.tenKhoaHoc}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div className="flex space-x-4 pt-4">
                                    <button
                                        type="submit"
                                        className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                                    >
                                        Thêm mới
                                    </button>
                                    <button
                                        type="button"
                                        onClick={() => setShowModal(false)}
                                        className="flex-1 bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors"
                                    >
                                        Hủy
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
