// Microservice URLs (inside Docker network)
const AUTH_SERVICE_URL = 'http://authentication:80/api';
const USER_SERVICE_URL = 'http://userservice:80/api';
const CHAT_SERVICE_URL = 'http://chatting:80/api';

interface FetchOptions extends RequestInit {
  skipAuth?: boolean;
}

/**
 * Determines the correct base URL based on the endpoint
 */
const getBaseUrl = (endpoint: string): string => {
  if (endpoint.startsWith('/Authentication')) {
    return AUTH_SERVICE_URL;
  } else if (endpoint.startsWith('/Users')) {
    return USER_SERVICE_URL;
  } else if (endpoint.startsWith('/Chat')) {
    return CHAT_SERVICE_URL;
  }
  return AUTH_SERVICE_URL;
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