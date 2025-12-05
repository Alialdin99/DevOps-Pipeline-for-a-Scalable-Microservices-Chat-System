// Use environment variables from Vite (set in docker-compose.yml)
// const AUTH_SERVICE_URL = import.meta.env.VITE_AUTH_SERVICE_URL || 'http://localhost:5000';
// const USER_SERVICE_URL = import.meta.env.VITE_USER_SERVICE_URL || 'http://localhost:5001';
// const CHAT_SERVICE_URL = import.meta.env.VITE_CHAT_SERVICE_URL || 'http://localhost:5002';
// const NOTIFICATION_SERVICE_URL = import.meta.env.VITE_NOTIFICATION_SERVICE_URL || 'http://localhost:5003';
// Use empty strings for environment variables since we're using relative paths
const BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

interface FetchOptions extends RequestInit {
  skipAuth?: boolean;
}

/**
 * API interceptor that automatically adds auth token to requests
 */
export const apiRequest = async (
  endpoint: string,
  options: FetchOptions = {}
): Promise<Response> => {
  const { skipAuth = false, headers = {}, ...restOptions } = options;

  const config: RequestInit = {
    ...restOptions,
    headers: {
      'Content-Type': 'application/json',
      ...headers,
    },
  };

  // Add authorization token if not skipped
  if (!skipAuth) {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers = {
        ...config.headers,
        Authorization: `Bearer ${token}`,
      };
    }
  }

  // Build the full URL
  // If endpoint starts with http, use it as is (absolute URL)
  // Otherwise, prepend BASE_URL (for relative paths through Ingress)
  const url = endpoint.startsWith('http')
    ? endpoint
    : `${BASE_URL}${endpoint}`;

  return fetch(url, config);
};

/**
 * Helper function to handle API responses and errors
 */
export const handleApiResponse = async <T>(response: Response): Promise<T> => {
  if (!response.ok) {
    try {
      const error = await response.json();
      throw new Error(error.message || 'An error occurred');
    } catch {
      throw new Error('Network error. Please check your connection.');
    }
  }

  const text = await response.text();
  if (!text) return null as T;

  try {
    return JSON.parse(text) as T;
  } catch {
    return text as T;
  }
};

/**
 * Get user info from token
 */
export const getCurrentUser = (): {
  id: string;
  username: string;
  email: string;
} | null => {
  const token = localStorage.getItem('authToken');
  if (!token) return null;

  try {
    const payload = token.split('.')[1];
    const decoded = JSON.parse(atob(payload));
    return {
      id: decoded.id || '',
      username: decoded.username || decoded.unique_name || decoded.sub || '',
      email: decoded.email || '',
    };
  } catch {
    return null;
  }
};