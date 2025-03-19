import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, timer } from 'rxjs';
import { io, Socket } from 'socket.io-client';
import { retryWhen, delayWhen, take } from 'rxjs/operators';

export interface StockUpdate {
  symbol: string;
  price: number;
  change: number;
  lastUpdate?: Date;
  name?: string;
}

@Injectable({
  providedIn: 'root',
})
export class WebSocketService {
  private socket!: Socket;
  private stockUpdatesSubject = new BehaviorSubject<StockUpdate | null>(null);
  private allStockUpdatesSubject = new BehaviorSubject<StockUpdate[]>([]);
  private isConnected = false;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;

  constructor() {
    this.initializeSocket();
  }

  private initializeSocket() {
    this.socket = io('http://localhost:8081', {
      reconnection: true,
      reconnectionAttempts: this.maxReconnectAttempts,
      reconnectionDelay: 1000,
      timeout: 20000,
      transports: ['websocket', 'polling'],
      withCredentials: true,
    });

    this.socket.on('connect', () => {
      console.log('Connected to WebSocket server');
      this.isConnected = true;
      this.reconnectAttempts = 0;
    });

    this.socket.on('disconnect', () => {
      console.log('Disconnected from WebSocket server');
      this.isConnected = false;
    });

    this.socket.on('connect_error', (error) => {
      console.error('WebSocket connection error:', error);
      this.isConnected = false;
      this.handleReconnect();
    });

    this.socket.on('stockUpdate', (data: StockUpdate) => {
      console.log('Received stock update:', data);
      this.stockUpdatesSubject.next(data);
    });

    this.socket.on('allStockUpdates', (data: StockUpdate[]) => {
      console.log('Received all stock updates:', data);
      this.allStockUpdatesSubject.next(data);
    });
  }

  private handleReconnect() {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      this.reconnectAttempts++;
      console.log(
        `Attempting to reconnect (${this.reconnectAttempts}/${this.maxReconnectAttempts})...`
      );
      timer(1000 * this.reconnectAttempts).subscribe(() => {
        this.socket.connect();
      });
    } else {
      console.error('Max reconnection attempts reached');
    }
  }

  subscribeToStocks(symbols: string[]): void {
    if (this.isConnected && symbols.length > 0) {
      console.log('Subscribing to stocks:', symbols);
      this.socket.emit('subscribe', symbols);
    } else if (!this.isConnected) {
      console.warn('Socket not connected, attempting to reconnect...');
      this.socket.connect();
    }
  }

  unsubscribeFromStocks(symbols: string[]): void {
    if (this.isConnected && symbols.length > 0) {
      console.log('Unsubscribing from stocks:', symbols);
      this.socket.emit('unsubscribe', symbols);
    }
  }

  getStockUpdates(): Observable<StockUpdate | null> {
    return this.stockUpdatesSubject.asObservable().pipe(
      retryWhen((errors) =>
        errors.pipe(
          delayWhen(() => timer(1000)),
          take(this.maxReconnectAttempts)
        )
      )
    );
  }
}
