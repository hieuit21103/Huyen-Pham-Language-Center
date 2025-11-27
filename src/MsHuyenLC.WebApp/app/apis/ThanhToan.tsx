import { getJwtToken } from ".";
import { ThanhToanApiUrl } from "../constants/apis-url";

export interface ThanhToan {
  id?: string;
  maThanhToan?: string;
  dangKyId?: string;
  hocVienId?: string;
  tenHocVien?: string;
  khoaHocId?: string;
  tenKhoaHoc?: string;
  soTien?: number;
  phuongThuc?: number;
  trangThai?: number;
  ghiChu?: string;
  thongTinNganHang?: string;
  maGiaoDichNganHang?: string;
  congThanhToan?: string;
  ngayLap?: string;
  ngayHetHan?: string;
  ngayThanhToan?: string;
}

export interface CreatePaymentUrlRequest {
  orderId?: string;
  amount?: number;
  orderInfo?: string;
  returnUrl?: string;
  ipAddress?: string;
  bankCode?: string;
}

export interface PaymentUrlResponse {
  success?: boolean;
  paymentUrl?: string;
  errorMessage?: string;
}

export interface VNPayCallbackRequest {
  vnp_TmnCode?: string;
  vnp_Amount?: string;
  vnp_BankCode?: string;
  vnp_BankTranNo?: string;
  vnp_CardType?: string;
  vnp_OrderInfo?: string;
  vnp_TransactionNo?: string;
  vnp_TransactionStatus?: string;
  vnp_PayDate?: string;
  vnp_ResponseCode?: string;
  vnp_TxnRef?: string;
  vnp_SecureHash?: string;
  vnp_SecureHashType?: string;
}

export async function getThanhToans(): Promise<{ success: boolean; data?: ThanhToan[]; message?: string }> {
  try {
    const token = getJwtToken();
    const response = await fetch(
      ThanhToanApiUrl(),
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );

    const data = await response.json();
    return data;
  } catch (error) {
    return { success: false, message: "Lỗi khi tải danh sách thanh toán" };
  }
}

export async function getThanhToansByStudentId(
  studentId: string
): Promise<{ success: boolean; data?: ThanhToan[]; message?: string }> {
  try {
    const token = getJwtToken();
    const response = await fetch(ThanhToanApiUrl(`student/${studentId}`), {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    const data = await response.json();
    return data;
  } catch (error) {
    return { success: false, message: "Lỗi khi tải danh sách thanh toán" };
  }
}

export async function getThanhToan(
  id: string
): Promise<{ success: boolean; data?: ThanhToan; message?: string }> {
  try {
    const token = getJwtToken();
    const response = await fetch(ThanhToanApiUrl(`/${id}`), {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    const data = await response.json();
    return data;
  } catch (error) {
    return { success: false, message: "Lỗi khi tải thông tin thanh toán" };
  }
}

export async function createThanhToan(
  thanhToan: ThanhToan
): Promise<{ success: boolean; data?: ThanhToan; message?: string }> {
  try {
    const token = getJwtToken();
    const response = await fetch(ThanhToanApiUrl(), {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(thanhToan),
    });

    const data = await response.json();
    return data;
  } catch (error) {
    return { success: false, message: "Lỗi khi tạo thanh toán" };
  }
}

export async function createVNPayUrl(
  id: string,
  returnUrl: string
): Promise<{ success: boolean; data?: PaymentUrlResponse; message?: string }> {
  try {
    const token = getJwtToken();
    const response = await fetch(
      ThanhToanApiUrl(`create/${id}?returnUrl=${encodeURIComponent(returnUrl)}`),
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );

    const data = await response.json();
    return data;
  } catch (error) {
    return { success: false, message: "Lỗi khi tạo URL thanh toán VNPay" };
  }
}

export async function getThanhToanByDangKyId(
  dangKyId: string
): Promise<{ success: boolean; data?: ThanhToan; message?: string }> {
  try {
    const token = getJwtToken();
    const response = await fetch(
      ThanhToanApiUrl("dang-ky/" + dangKyId),
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );

    return await response.json();
  } catch (error) {
    return { success: false, message: "Không tìm thấy thanh toán" };
  }
}

export async function callback(
  VPay: VNPayCallbackRequest
): Promise<{ success: boolean; message?: string }> {
  try {
    const token = getJwtToken();
    const response = await fetch(ThanhToanApiUrl(`return`), {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(VPay),
    });

    const data = await response.json();
    return data;
  } catch (error) {
    return { success: false, message: "Lỗi khi gọi lại thanh toán" };
  }
}
