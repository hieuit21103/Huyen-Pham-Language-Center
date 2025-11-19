import { PhienLamBaiApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    SubmitRequest, 
    GradingRequest,
    ApiResponse 
} from "~/types/index";

/**
 * Nộp bài thi
 * Hệ thống sẽ tự động chấm điểm cho các câu trắc nghiệm
 */
export async function submitBaiThi(request: SubmitRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl('submit'), {
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
 * Chấm bài thi (dành cho giáo viên)
 * Tổng điểm sẽ được tự động tính từ tổng điểm các câu hỏi
 */
export async function gradeBaiThi(id: string, request: GradingRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(`${id}/grade`), {
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
 * Chấm điểm phiên làm bài (alias cho gradeBaiThi)
 */
export async function gradePhienLamBai(id: string, request: GradingRequest): Promise<ApiResponse> {
    return gradeBaiThi(id, request);
}

/**
 * Lấy chi tiết bài thi và các câu trả lời
 */
export async function getPhienLamBaiDetails(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(`${id}/details`), {
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
 * Lấy danh sách bài thi của học viên
 */
export async function getPhienLamBaiByHocVien(hocVienId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(`hocvien/${hocVienId}`), {
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
 * Lấy danh sách bài thi theo đề thi
 */
export async function getPhienLamBaiByDeThi(deThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(`dethi/${deThiId}`), {
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
 * Lấy danh sách phiên làm bài
 */
export async function getPhienLamBais(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(), {
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
 * Lấy phiên làm bài theo ID
 */
export async function getPhienLamBai(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(id), {
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
 * Lấy phiên làm bài theo ID (alias)
 */
export async function getPhienLamBaiById(id: string): Promise<ApiResponse> {
    return getPhienLamBai(id);
}

/**
 * Lấy thống kê kết quả bài thi của học viên
 */
export async function getStatisticsByHocVien(hocVienId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(`statistics/hocvien/${hocVienId}`), {
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
 * Xuất kết quả bài thi ra file
 */
export async function exportResult(id: string, format: 'pdf' | 'excel' = 'pdf'): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhienLamBaiApiUrl(`${id}/export?format=${format}`), {
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
