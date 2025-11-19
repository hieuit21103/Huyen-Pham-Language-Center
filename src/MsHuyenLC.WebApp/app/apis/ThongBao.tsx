import { getJwtToken } from "./Auth";
import type { ApiResponse } from "~/types/index";
import { NotificationApiUrl } from "~/constants/apis-url";
import type { ThongBaoRequest, ThongBaoResponse } from "~/types/notification.types";

export const getThongBaos = async (): Promise<ApiResponse<ThongBaoResponse[]>> => {
  try {
    const token = getJwtToken();

    const response = await fetch(`${NotificationApiUrl()}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi lấy danh sách thông báo" };
  }
};

export const getThongBaoById = async (id: string): Promise<ApiResponse<ThongBaoResponse>> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${NotificationApiUrl()}/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi lấy thông tin thông báo" };
  }
};

export const createThongBao = async (data: ThongBaoRequest): Promise<ApiResponse<ThongBaoResponse>> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${NotificationApiUrl()}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
      body: JSON.stringify(data),
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi tạo thông báo" };
  }
};

export const updateThongBao = async (id: string, data: ThongBaoRequest): Promise<ApiResponse<ThongBaoResponse>> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${NotificationApiUrl()}/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
      body: JSON.stringify(data),
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi cập nhật thông báo" };
  }
};

export const deleteThongBao = async (id: string): Promise<ApiResponse> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${NotificationApiUrl()}/${id}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi xóa thông báo" };
  }
};

export const getThongBaoByTaiKhoanId = async (taiKhoanId: string): Promise<ApiResponse<ThongBaoResponse[]>> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${NotificationApiUrl()}/nguoi-nhan/${taiKhoanId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi lấy thông báo theo tài khoản" };
  }
};