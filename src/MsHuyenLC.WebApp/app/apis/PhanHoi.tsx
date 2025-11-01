import { getJwtToken } from "./Auth";
import type { ApiResponse } from "~/types/index";
import { ReportApiUrl } from "~/constants/apis-url";

export interface PhanHoiRequest {
  hocVienId?: string;
  loaiPhanHoi?: string;
  tieuDe?: string;
  noiDung?: string;
}

export interface PhanHoiResponse {
  id?: string;
  hocVienId?: string;
  tenHocVien?: string;
  loaiPhanHoi?: string;
  tieuDe?: string;
  noiDung?: string;
  ngayTao?: string;
}

export const getPhanHois = async (
  pageNumber: number = 1,
  pageSize: number = 10,
  sortBy?: string,
  sortOrder: string = "asc"
): Promise<ApiResponse<PhanHoiResponse[]>> => {
  try {
    const token = getJwtToken();
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });
    
    if (sortBy) params.append("sortBy", sortBy);
    if (sortOrder) params.append("sortOrder", sortOrder);

    const response = await fetch(`${ReportApiUrl()}?${params.toString()}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
    });

        return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi lấy thông tin phản hồi" };
  }
};

export const createPhanHoi = async (data: PhanHoiRequest): Promise<ApiResponse<PhanHoiResponse>> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${ReportApiUrl()}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
      body: JSON.stringify(data),
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi tạo phản hồi" };
  }
};

export const updatePhanHoi = async (id: string, data: PhanHoiRequest): Promise<ApiResponse<PhanHoiResponse>> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${ReportApiUrl()}/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
      body: JSON.stringify(data),
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi cập nhật phản hồi" };
  }
};

export const deletePhanHoi = async (id: string): Promise<ApiResponse> => {
  try {
    const token = getJwtToken();
    const response = await fetch(`${ReportApiUrl()}/${id}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        ...(token && { "Authorization": `Bearer ${token}` }),
      },
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: "Có lỗi xảy ra khi xóa phản hồi" };
  }
};
