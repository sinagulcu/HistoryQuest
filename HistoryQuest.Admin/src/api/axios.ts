import axios from "axios";

const resolveApiBaseUrl = () => {
  const envBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim();

  if (!envBaseUrl) {
    return "https://localhost:7086/api";
  }

  // Common local mismatch: backend HTTP on 5007 but URL configured as HTTPS.
  if (envBaseUrl.includes("https://localhost:5007")) {
    return envBaseUrl.replace("https://localhost:5007", "http://localhost:5007");
  }

  return envBaseUrl;
};

const API_BASE_URL = resolveApiBaseUrl();

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: false,
});

api.interceptors.request.use(
  (config) => {
    if (config.skipAuth) {
      return config;
    }

    const token = localStorage.getItem("historyquest_token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const requestUrl = String(error.config?.url ?? "");
    const skipAuthRedirect = Boolean(error.config?.skipAuth);
    const autoLogoutOn401 = Boolean(error.config?.autoLogoutOn401);
    const isAuthEndpoint = requestUrl.toLowerCase().includes("/auth/login") || requestUrl.toLowerCase().includes("/auth/register");

    if (error.response?.status === 401) {
      if (skipAuthRedirect) {
        return Promise.reject(error);
      }

      // Login/register 401 response should be handled by the page itself.
      if (isAuthEndpoint) {
        return Promise.reject(error);
      }

      // Some backends return 401 for role-restricted endpoints.
      // Avoid global redirect loops unless explicitly requested.
      if (autoLogoutOn401) {
        localStorage.removeItem("historyquest_token");
        localStorage.removeItem("historyquest_user");
        localStorage.removeItem("historyquest-auth");
        window.location.href = "/login";
      }
    }

    return Promise.reject(error);
  }
);

export default api;