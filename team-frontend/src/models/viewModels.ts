import { GameState, QuizItemType, Team, Game, QuizItem, InteractionType, ChoiceOption } from './models';

export interface TeamViewModel {
    /** The team Id */
    id: string;
    /** The Team Name */
    name: string;
    /** Team member names */
    memberNames: string;
    /** false when the user has logged out / left game */
    isLoggedIn: boolean;
  }

export interface TeamLobbyViewModel {
    game: Game;
    otherTeamsInGame: TeamViewModel[];
}

export interface TeamInGameViewModel {
    game: Game;
    quizItemViewModel: QuizItemViewModel;
}

export interface QuizItemViewModel  {
    id: string;
    title: string;
    body: string;
    media: []// MediaObject[];
    quizItemType: QuizItemType;
    maxScore: number;
    interactions: InteractionViewModel[];
}

export interface InteractionViewModel {
    id: string;
    text: string;
    interactionType: InteractionType;
    choiceOptions: ChoiceOption[];
    maxScore: number;
    response: string;
    chosenOptions: number[];
    chosenOption: number;  
}

export interface QmLobbyViewModel {
    userId: string;
    game: Game;
    teamsInGame: Team[];
}

export interface QmInGameViewModel {
    qmTeamFeed: TeamFeedViewModel;
    qmTeamRanking: TeamRankingViewModel;
    game: Game;
    currentQuizItem: QuizItem;
    // teamfeed
    // game info
    // current quiz itm
    // ranking
}

export interface TeamFeedViewModel {
    teams: Team[];
}

export interface TeamRankingViewModel {
    teams: Team[];
}