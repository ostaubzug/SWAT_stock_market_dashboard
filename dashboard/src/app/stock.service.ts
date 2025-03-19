import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Stock {
  symbol: string;
  name: string;
  price: number;
  change: number;
  data?: Array<{ date: Date; value: number }>;
}

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private apiUrl = 'http://localhost:8080/api/stocks';

  constructor(private http: HttpClient) {}

  getUserStocks(): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.apiUrl}/user-stocks`);
  }

  saveUserStocks(stocks: Stock[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/user-stocks`, { stocks });
  }

  searchStocks(query: string): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.apiUrl}/search?q=${query}`);
  }

  getStockDetails(symbol: string): Observable<Stock> {
    return this.http.get<Stock>(`${this.apiUrl}/details/${symbol}`);
  }
}
