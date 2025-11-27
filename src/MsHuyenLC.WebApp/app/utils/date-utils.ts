/**
 * Format date to yyyy-MM-dd for input[type="date"]
 * @param dateString - Date string from API (ISO format)
 * @returns Formatted date string in yyyy-MM-dd format
 */
export function formatDateForInput(dateString?: string | null): string {
  if (!dateString) return "";
  
  try {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    
    return `${year}-${month}-${day}`;
  } catch (error) {
    console.error('Error formatting date:', error);
    return "";
  }
}

/**
 * Format date for display (Vietnamese locale)
 * @param dateString - Date string from API (ISO format)
 * @returns Formatted date string for display
 */
export function formatDateForDisplay(dateString?: string | null): string {
  if (!dateString) return "—";
  
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  } catch (error) {
    console.error('Error formatting date for display:', error);
    return "—";
  }
}

/**
 * Format date and time for display (Vietnamese locale)
 * @param dateString - Date string from API (ISO format)
 * @returns Formatted date and time string for display
 */
export function formatDateTime(dateString?: string | null): string {
  if (!dateString) return "—";
  
  try {
    const date = new Date(dateString);
    return date.toLocaleString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  } catch (error) {
    console.error('Error formatting date time:', error);
    return "—";
  }
}

