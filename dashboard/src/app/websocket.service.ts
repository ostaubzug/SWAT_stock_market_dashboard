import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { io, Socket } from 'socket.io-client';

export interface StockUpdate {
  symbol: string;
  price: number;
  change: number;
  lastUpdate?: Date;
  name?: string;
}

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private socket: Socket;
  private stockUpdatesSubject = new BehaviorSubject<StockUpdate | null>(null);
  private allStockUpdatesSubject = new BehaviorSubject<StockUpdate[]>([]);
  private isConnected = false;

  constructor() {
    this.socket = io('http://localhost:8081');

    this.socket.on('connect', () => {
      console.log('Connected to WebSocket server');
      this.isConnected = true;
    });

    this.socket.on('disconnect', () => {
      console.log('Disconnected from WebSocket server');
      this.isConnected = false;
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

  subscribeToStocks(symbols: string[]): void {
    if (this.isConnected && symbols.length > 0) {
      console.log('Subscribing to stocks:', symbols);
      this.socket.emit('subscribe', symbols);
    }
  }

  unsubscribeFromStocks(symbols: string[]): void {
    if (this.isConnected && symbols.length > 0) {
      console.log('Unsubscribing from stocks:', symbols);
      this.socket.emit('unsubscribe', symbols);
    }
  }

  getStockUpdates(): Observable<StockUpdate | null> {
    return this.stockUpdatesSubject.asObservable();
  }
}
