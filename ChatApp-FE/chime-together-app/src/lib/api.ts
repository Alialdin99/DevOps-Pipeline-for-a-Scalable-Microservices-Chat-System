// Use environment variables from Vite (set in docker-compose.yml)
const AUTH_SERVICE_URL = import.meta.env.VITE_AUTH_SERVICE_URL || 'http://localhost:5000';
const USER_SERVICE_URL = import.meta.env.VITE_USER_SERVICE_URL || 'http://localhost:5001';
const CHAT_SERVICE_URL = import.meta.env.VITE_CHAT_SERVICE_URL || 'http://localhost:5002';
const NOTIFICATION_SERVICE_URL = import.meta.env.VITE_NOTIFICATION_SERVICE_URL || 'http://localhost:5003';

interface FetchOptions extends RequestInit {
  skipAuth?: boolean;
}

/**
 * Determines the correct base URL based on the endpoint
 */
const getBaseUrl = (endpoint: string): string => {
  if (endpoint.startsWith('/Authentication')) return `${AUTH_SERVICE_URL}/api`;
  if (endpoint.startsWith('/Users')) return `${USER_SERVICE_URL}/api`;
  if (endpoint.startsWith('/Chat')) return `${CHAT_SERVICE_URL}/api`;
  if (endpoint.startsWith('/Notifications')) return `${NOTIFICATION_SERVICE_URL}/api`;
  return `${AUTH_SERVICE_URL}/api`; // default fallback
};

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

  if (!skipAuth) {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers = {
        ...config.headers,
        Authorization: `Bearer ${token}`,
      };
    }
  }

  const url = endpoint.startsWith('http')
    ? endpoint
    : `${getBaseUrl(endpoint)}${endpoint}`;

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
