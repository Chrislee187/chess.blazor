import { ChessBoard } from "../../models/ChessBoard";

import { Component, OnInit, Input  } from '@angular/core';
import { ChessBoardService } from "../../services/chess-board.service";
import { PgnJson } from "../../models/PgnJson";

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent implements OnInit {
  @Input() boardKey: string;
  @Input() game : PgnJson;
  constructor(private chessBoardService: ChessBoardService) { }

  
  private chessBoard : ChessBoard;
  
  ngOnInit() {
    this.chessBoard = this.chessBoardService.get(this.boardKey);
  }
}

