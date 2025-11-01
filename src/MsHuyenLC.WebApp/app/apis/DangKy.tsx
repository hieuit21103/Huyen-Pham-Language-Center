import { DangKyApiUrl, DangKyKhachApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    DangKyRequest,
    DangKyCreateRequest,
    DangKyUpdateRequest,
    DangKyKhachRequest,
    DangKyKhachCreateRequest,
    DangKyKhachUpdateRequest,
    PaginationParams,
    ApiResponse 
} from "~/types/index";

// ===== ĐĂNG KÝ HỌC VIÊN =====

/**
 * Đăng ký học viên vào khóa học (cho học viên tự đăng ký)
 */
export async function registerStudent(request: DangKyRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyApiUrl('student'), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Tạo đăng ký mới (Admin/GiaoVu)
 */
export async function createDangKy(request: DangKyCreateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyApiUrl(), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Cập nhật đăng ký
 */
export async function updateDangKy(id: string, request: DangKyUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyApiUrl(id), {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xóa đăng ký
 */
export async function deleteDangKy(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyApiUrl(id), {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy đăng ký theo ID
 */
export async function getDangKy(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyApiUrl(id), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy danh sách đăng ký (có phân trang)
 */
export async function getDangKys(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${DangKyApiUrl()}?${queryParams.toString()}`
            : DangKyApiUrl();

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy danh sách đăng ký theo học viên
 */
export async function getDangKysByHocVien(hocVienId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyApiUrl(`student/${hocVienId}`), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

// ===== ĐĂNG KÝ KHÁCH =====

/**
 * Đăng ký khách tự đăng ký (Public - không cần authentication)
 */
export async function registerGuest(request: DangKyKhachRequest): Promise<ApiResponse> {
    try {
        const response = await fetch(DangKyKhachApiUrl('register'), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Admin/GiaoVu tạo đăng ký khách sau khi tư vấn
 */
export async function createDangKyKhach(request: DangKyKhachCreateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyKhachApiUrl('create'), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Cập nhật đăng ký khách
 */
export async function updateDangKyKhach(id: string, request: DangKyKhachUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyKhachApiUrl(id), {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xóa đăng ký khách
 */
export async function deleteDangKyKhach(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyKhachApiUrl(id), {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy đăng ký khách theo ID
 */
export async function getDangKyKhach(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyKhachApiUrl(id), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy danh sách đăng ký khách (có phân trang)
 */
export async function getDangKyKhachs(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${DangKyKhachApiUrl()}?${queryParams.toString()}`
            : DangKyKhachApiUrl();

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Tìm kiếm đăng ký khách
 */
export async function searchDangKyKhach(keyword?: string, trangThai?: number, ketQua?: number): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (keyword) queryParams.append('keyword', keyword);
        if (trangThai !== undefined) queryParams.append('trangThai', trangThai.toString());
        if (ketQua !== undefined) queryParams.append('ketQua', ketQua.toString());

        const url = queryParams.toString() 
            ? `${DangKyKhachApiUrl('search')}?${queryParams.toString()}`
            : DangKyKhachApiUrl('search');

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy thống kê đăng ký khách cho dashboard
 */
export async function getDangKyKhachStatistics(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DangKyKhachApiUrl('statistics'), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}
