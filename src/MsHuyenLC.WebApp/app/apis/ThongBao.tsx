import { getJwtToken } from "./Auth";
import type { ApiResponse } from "~/types/index";
import { NotificationApiUrl } from "~/constants/apis-url";

export interface ThongBaoRequest {
  tieuDe: string;
  noiDung: string;
}

export interface ThongBaoResponse {
  id?: string;
  nguoiGuiId?: string;
  tieuDe?: string;
  noiDung?: string;
  ngayTao?: string;
}

export interface ThongBaoNguoiNhanResponse {
  tenNguoiGui?: string;
  tieuDe?: string;
  noiDung?: string;
  ngayTao?: string;
}

// Lấy tất cả thông báo (GetAll) - Trả về danh sách ThongBaoNguoiNhanResponse
export const getThongBaos = async (
  pageNumber: number = 1,
  pageSize: number = 10,
  sortBy?: string,
  sortOrder: string = "desc"
): Promise<ApiResponse<ThongBaoNguoiNhanResponse[]>> => {
  try {
    const token = getJwtToken();
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });
    
    if (sortBy) params.append("sortBy", sortBy);
    if (sortOrder) params.append("sortOrder", sortOrder);

    const response = await fetch(`${NotificationApiUrl()}?${params.toString()}`, {
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
