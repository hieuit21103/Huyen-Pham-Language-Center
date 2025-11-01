import { Search, Download, Filter, X, BarChart3, TrendingUp, Award, Clock, Calendar, User, FileText } from "lucide-react";
import { useState, useEffect } from "react";
import { getPhienLamBais } from "~/apis/PhienLamBai";
import { getHocViens } from "~/apis/HocVien";
import Pagination from "~/components/Pagination";
import type { PhienLamBai } from "~/types/exam.types";
import { setLightTheme } from "./_layout";

interface HocVien {
  id: string;
  hoTen: string;
  email?: string;
}

export default function AdminBaoCaoKetQua() {
  const [phienLamBais, setPhienLamBais] = useState<PhienLamBai[]>([]);
  const [hocViens, setHocViens] = useState<HocVien[]>([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");
  
  // Filters
  const [selectedHocVien, setSelectedHocVien] = useState("");
  const [searchHocVien, setSearchHocVien] = useState("");
  const [tuNgay, setTuNgay] = useState("");
  const [denNgay, setDenNgay] = useState("");
  const [showFilter, setShowFilter] = useState(false);
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);

  useEffect(() => {
    loadHocViens();
    loadPhienLamBais();
  }, [currentPage]);

  useEffect(() => {
    if (selectedHocVien || tuNgay || denNgay) {
      loadPhienLamBais();
      setLightTheme();
    }
  }, [selectedHocVien, tuNgay, denNgay]);

  const loadHocViens = async () => {
    const response = await getHocViens({ pageSize: 1000 });
    if (response.success && Array.isArray(response.data)) {
      setHocViens(response.data);
    }
  };

  const loadPhienLamBais = async () => {
    setLoading(true);
    const response = await getPhienLamBais({
      pageNumber: currentPage,
      pageSize: pageSize,
      sortBy: 'thoiGianBatDau',
      sortOrder: 'desc'
    });
    
    if (response.success && Array.isArray(response.data)) {
      setPhienLamBais(response.data);
      setTotalCount((response as any).totalCount || response.data.length);
    }
    setLoading(false);
  };

  const handleResetFilter = () => {
    setSelectedHocVien("");
    setSearchHocVien("");
    setTuNgay("");
    setDenNgay("");
    setCurrentPage(1);
  };

  const filteredPhienLamBais = phienLamBais.filter(plb => {
    const matchHocVien = !selectedHocVien || plb.hocVienId === selectedHocVien;

    const matchTuNgay = !tuNgay || (plb.ngayLam && new Date(plb.ngayLam) >= new Date(tuNgay));
    const matchDenNgay = !denNgay || (plb.ngayLam && new Date(plb.ngayLam) <= new Date(denNgay + 'T23:59:59'));

    return matchHocVien && matchTuNgay && matchDenNgay;
  });

  const filteredHocViens = hocViens.filter(hv =>
    hv.hoTen?.toLowerCase().includes(searchHocVien.toLowerCase())
  );

  const formatDate = (dateString?: string) => {
    if (!dateString) return "—";
    const date = new Date(dateString);
    return date.toLocaleString('vi-VN');
  };

  const formatShortDate = (dateString?: string) => {
    if (!dateString) return "—";
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  };

  const getDiemColor = (diem?: number, tongCauHoi?: number) => {
    if (!diem || !tongCauHoi) return "text-gray-600";
    const percent = (diem / (tongCauHoi * 10)) * 100; // Assuming 10 points per question max
    if (percent >= 80) return "text-green-600 font-bold";
    if (percent >= 50) return "text-yellow-600 font-semibold";
    return "text-red-600 font-semibold";
  };

  const stats = {
    tongBaiThi: filteredPhienLamBais.length,
    daHoanThanh: filteredPhienLamBais.filter(p => p.diem !== undefined && p.diem !== null).length,
    diemTrungBinh: filteredPhienLamBais.filter(p => p.diem !== undefined && p.diem !== null).length > 0
      ? (filteredPhienLamBais.reduce((sum, p) => sum + (p.diem || 0), 0) / 
         filteredPhienLamBais.filter(p => p.diem !== undefined && p.diem !== null).length).toFixed(2)
      : 0,
    tyLeDat: filteredPhienLamBais.filter(p => p.diem !== undefined && p.diem !== null).length > 0
      ? ((filteredPhienLamBais.filter(p => p.diem !== undefined && p.diem !== null && p.tongCauHoi && (p.diem / (p.tongCauHoi * 10)) >= 0.5).length / 
          filteredPhienLamBais.filter(p => p.diem !== undefined && p.diem !== null).length) * 100).toFixed(1)
      : 0,
  };

  return (
    <div className="space-y-6">
      {message && (
        <div className={`${message.includes("thành công") ? "bg-green-100 border-green-400 text-green-700" : "bg-red-100 border-red-400 text-red-700"} border px-4 py-3 rounded-lg`}>
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Báo cáo kết quả học tập</h1>
          <p className="text-gray-600 mt-1">Theo dõi và phân tích kết quả thi của học viên</p>
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => setShowFilter(!showFilter)}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
          >
            <Filter className="w-5 h-5" />
            <span>Bộ lọc</span>
          </button>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-blue-700 text-sm font-medium mb-1">Tổng bài thi</p>
              <p className="text-2xl font-bold text-blue-900">{stats.tongBaiThi}</p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <FileText className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div className="bg-green-50 border border-green-200 rounded-lg p-4">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-green-700 text-sm font-medium mb-1">Đã hoàn thành</p>
              <p className="text-2xl font-bold text-green-900">{stats.daHoanThanh}</p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <Award className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-purple-700 text-sm font-medium mb-1">Điểm trung bình</p>
              <p className="text-2xl font-bold text-purple-900">{stats.diemTrungBinh}</p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </div>

        <div className="bg-orange-50 border border-orange-200 rounded-lg p-4">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-orange-700 text-sm font-medium mb-1">Tỷ lệ đạt</p>
              <p className="text-2xl font-bold text-orange-900">{stats.tyLeDat}%</p>
            </div>
            <div className="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
              <BarChart3 className="w-6 h-6 text-orange-600" />
            </div>
          </div>
        </div>
      </div>

      {/* Filter Panel */}
      {showFilter && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <div className="flex justify-between items-center mb-4">
            <h3 className="font-semibold text-gray-900">Bộ lọc</h3>
            <button onClick={() => setShowFilter(false)} className="text-gray-500 hover:text-gray-700">
              <X className="w-5 h-5" />
            </button>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Học viên</label>
              <div className="relative">
                <input
                  type="text"
                  placeholder="Tìm kiếm học viên..."
                  value={searchHocVien}
                  onChange={(e) => setSearchHocVien(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900 mb-2"
                />
                <select
                  value={selectedHocVien}
                  onChange={(e) => {
                    setSelectedHocVien(e.target.value);
                    setCurrentPage(1);
                  }}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                >
                  <option value="">-- Tất cả học viên --</option>
                  {filteredHocViens.map((hv) => (
                    <option key={hv.id} value={hv.id}>
                      {hv.hoTen}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Từ ngày</label>
              <input
                type="date"
                value={tuNgay}
                onChange={(e) => {
                  setTuNgay(e.target.value);
                  setCurrentPage(1);
                }}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Đến ngày</label>
              <input
                type="date"
                value={denNgay}
                onChange={(e) => {
                  setDenNgay(e.target.value);
                  setCurrentPage(1);
                }}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>
          </div>

          <div className="flex justify-end mt-4">
            <button
              onClick={handleResetFilter}
              className="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Đặt lại
            </button>
          </div>
        </div>
      )}

      {/* Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : filteredPhienLamBais.length === 0 ? (
          <div className="p-8 text-center">
            <BarChart3 className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Không tìm thấy kết quả nào</p>
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
                    Đề thi
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Ngày làm
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thời gian làm
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Điểm
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Số câu đúng
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredPhienLamBais.map((plb) => (
                  <tr key={plb.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                          <User className="w-5 h-5 text-blue-600" />
                        </div>
                        <div>
                          <p className="text-sm font-medium text-gray-900">
                            {plb.hocVien?.hoTen || "—"}
                          </p>
                          {plb.hocVien?.email && (
                            <p className="text-xs text-gray-500">{plb.hocVien.email}</p>
                          )}
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <p className="text-sm text-gray-900">{plb.deThi?.tenDe || "—"}</p>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-gray-600">
                        <div className="flex items-center">
                          <Calendar className="w-3 h-3 mr-1 text-gray-400" />
                          {formatShortDate(plb.ngayLam)}
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-gray-600">
                        {plb.thoiGianLam || "—"}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <p className={`text-sm font-semibold ${getDiemColor(plb.diem, plb.tongCauHoi)}`}>
                        {plb.diem !== undefined && plb.diem !== null ? plb.diem : "—"}
                      </p>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {plb.soCauDung !== undefined ? `${plb.soCauDung}/${plb.tongCauHoi || 0}` : "—"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Pagination */}
      {!loading && totalCount > 0 && (
        <div className="flex justify-center mt-6">
          <Pagination
            currentPage={currentPage}
            totalCount={totalCount}
            pageSize={pageSize}
            onPageChange={(page) => setCurrentPage(page)}
          />
        </div>
      )}
    </div>
  );
}
