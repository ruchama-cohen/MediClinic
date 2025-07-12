// src/utils/errorUtil.js
export function handleApiError(error) {
  if (error.response && error.response.data && error.response.data.message) {
    alert(error.response.data.message);
  } else if (error.message) {
    alert(`Error: ${error.message}`);
  } else {
    alert('An unexpected error occurred');
  }
  console.error('API Error:', error);
}