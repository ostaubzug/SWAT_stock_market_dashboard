import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

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

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }

  getUserStocks(): Observable<Stock[]> {
    const symbols = localStorage.getItem('userStocks') || '';
    return this.http
      .get<Stock[]>(`${this.apiUrl}/user-stocks?symbols=${symbols}`)
      .pipe(
        catchError(this.handleError),
        map((stocks) =>
          stocks.map((stock) => ({
            ...stock,
            price: Number(stock.price),
            change: Number(stock.change),
            data: stock.data?.map((point) => ({
              date: new Date(point.date),
              value: Number(point.value),
            })),
          }))
        )
      );
  }

  saveUserStocks(stocks: Stock[]): Observable<any> {
    const symbols = stocks.map((stock) => stock.symbol).join(',');
    localStorage.setItem('userStocks', symbols);
    return this.http
      .post(`${this.apiUrl}/user-stocks`, { stocks })
      .pipe(catchError(this.handleError));
  }

  searchStocks(query: string): Observable<Stock[]> {
    return this.http
      .get<Stock[]>(`${this.apiUrl}/search?q=${query}`)
      .pipe(catchError(this.handleError));
  }

  getStockDetails(symbol: string): Observable<Stock> {
    return this.http.get<Stock>(`${this.apiUrl}/details/${symbol}`).pipe(
      catchError(this.handleError),
      map((stock) => ({
        ...stock,
        price: Number(stock.price),
        change: Number(stock.change),
        data: stock.data?.map((point) => ({
          date: new Date(point.date),
          value: Number(point.value),
        })),
      }))
    );
  }
}
