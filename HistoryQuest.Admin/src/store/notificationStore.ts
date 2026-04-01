import { create } from "zustand";

export interface NotificationItem {
  id: string;
  title: string;
  read: boolean;
}

interface NotificationState {
  notifications: NotificationItem[];
  addNotification: (title: string) => void;
  markAsRead: (id: string) => void;
}

export const useNotificationStore = create<NotificationState>((set) => ({
  notifications: [],
  addNotification: (title) => {
    const newItem: NotificationItem = {
      id: crypto.randomUUID(),
      title,
      read: false,
    };

    set((state) => ({ notifications: [newItem, ...state.notifications] }));
  },
  markAsRead: (id) => {
    set((state) => ({
      notifications: state.notifications.map((item) =>
        item.id === id ? { ...item, read: true } : item
      ),
    }));
  },
}));