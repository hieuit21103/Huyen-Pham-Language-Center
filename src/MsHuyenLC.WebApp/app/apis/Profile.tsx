import { ProfileApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { ApiResponse } from "~/types/index";

/**
 * Lấy thông tin profile của người dùng hiện tại
 */
export async function getProfile(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        if (!token) {
            return { success: false, message: "Chưa đăng nhập" };
        }

        const response = await fetch(ProfileApiUrl(), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Cập nhật profile của người dùng hiện tại
 */
export async function updateProfile(request: any): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        if (!token) {
            return { success: false, message: "Chưa đăng nhập" };
        }

        const response = await fetch(ProfileApiUrl(), {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}
