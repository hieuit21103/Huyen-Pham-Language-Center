import { KyThiApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    KyThiRequest,
    KyThiUpdateRequest,
    TrangThaiKyThi,
    PaginationParams,
    ApiResponse 
} from "~/types/index";

export async function createKyThi(request: KyThiRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(), {
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
 * Cập nhật kỳ thi
 */
export async function updateKyThi(id: string, request: KyThiUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(id), {
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
 * Cập nhật trạng thái kỳ thi
 */
export async function updateKyThiStatus(id: string, trangThai: TrangThaiKyThi): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${id}/status`), {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(trangThai),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xóa kỳ thi
 */
export async function deleteKyThi(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(id), {
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
 * Lấy danh sách kỳ thi (có phân trang)
 */
export async function getKyThis(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${KyThiApiUrl()}?${queryParams.toString()}`
            : KyThiApiUrl();

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
 * Lấy danh sách kỳ thi theo lớp học
 */
export async function getKyThiByLopHoc(lopHocId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`lop/${lopHocId}`), {
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
 * Đăng ký học viên vào kỳ thi
 */
export async function registerHocVienForKyThi(kyThiId: string, hocVienIds: string[]): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/register`), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify({ hocVienIds }),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy danh sách học viên đã đăng ký kỳ thi
 */
export async function getRegisteredHocViens(kyThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/registered-students`), {
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
 * Lấy danh sách học viên đã nộp bài
 */
export async function getSubmittedHocViens(kyThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/submitted-students`), {
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
 * Lấy thống kê kết quả kỳ thi
 */
export async function getKyThiStatistics(kyThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/statistics`), {
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
 * Xuất báo cáo kỳ thi ra Excel
 */
export async function exportKyThiReport(kyThiId: string, format: 'excel' | 'pdf' = 'excel'): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/export?format=${format}`), {
            method: "GET",
            headers: {
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Kết thúc kỳ thi (cập nhật trạng thái sang "Kết thúc")
 */
export async function endKyThi(kyThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/end`), {
            method: "POST",
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
 * Hủy kỳ thi
 */
export async function cancelKyThi(kyThiId: string, reason?: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/cancel`), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify({ reason }),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy kết quả chi tiết của kỳ thi theo học viên
 */
export async function getKyThiResultByHocVien(kyThiId: string, hocVienId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KyThiApiUrl(`${kyThiId}/result/${hocVienId}`), {
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
