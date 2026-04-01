import { useCallback, useState } from "react";
import axios from "axios";

export const useApi = <T,>() => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [data, setData] = useState<T | null>(null);

  const execute = useCallback(async (request: () => Promise<T>) => {
    setLoading(true);
    setError(null);

    try {
      const response = await request();
      setData(response);
      return response;
    } catch (requestError) {
      if (axios.isAxiosError(requestError)) {
        const apiMessage = requestError.response?.data?.message;
        setError(typeof apiMessage === "string" ? apiMessage : "Beklenmeyen bir API hatası oluştu.");
      } else {
        setError("Beklenmeyen bir hata oluştu.");
      }
      throw requestError;
    } finally {
      setLoading(false);
    }
  }, []);

  return {
    loading,
    error,
    data,
    execute,
  };
};