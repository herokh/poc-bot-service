
var botstore = null;

const init = () => {
    initChatbot();
}

const initChatbot = () => {
    var usernameKey = 'u_name';
    var username = localStorage.getItem(usernameKey);
    if (!username) {
        username = prompt('please enter your name');
        localStorage.setItem(usernameKey, username);
    }

    $.ajax({
        url: '/api/Chatbot/Token',
        method: 'POST',
        Accept: 'application/json',
        contentType: 'application/json'
    }).done(token => createBotConnection(token, username));
}

const createBotConnection = (token, username) => {
    var botConnection = window.WebChat.createDirectLine({
        token: token.c,
        webSocket: false // defaults is true
    });

    renderWebChat(botConnection, token, username);
}

const searchFlight = (activity) => {
    var lastPath = location.href.split('/')[3];
    var isExecuted = localStorage.getItem('search_flight_executed');
    if (isExecuted && lastPath.indexOf('FlightBooking') !== -1) {
        localStorage.removeItem('search_flight_executed');
        searchFlightCompleted();
    }
    else if (!isExecuted) {
        localStorage.setItem('search_flight_executed', 1);
        var flightFilter = activity.value;
        location.href = `/FlightSearch?Origin=${flightFilter.Origin}&Destination=${flightFilter.Destination}&DepartureDate=${flightFilter.DepartureDate}&ArrivalDate=${flightFilter.DepartureDate}`;
    }
    else if (lastPath.indexOf('FlightSearch') !== -1) {
        $('.btn.btn-primary').click();
    }

}

const renderWebChat = (botConnection, token, username) => {
    var botElement = document.getElementById('webchat');

    botstore = window.WebChat.createStore({}, ({ dispatch }) => next => action => {
        if (action.type === 'DIRECT_LINE/INCOMING_ACTIVITY') {
            const event = new Event('webchatincomingactivity');
            event.data = action.payload.activity;
            window.dispatchEvent(event);
        }

        return next(action);
    });

    window.WebChat.renderWebChat({
        directLine: botConnection,
        store: botstore,
        userID: username,
        webSpeechPonyfillFactory: createCognitiveServicesSpeechServicesPonyfillFactory({
            credentials: {
                authorizationToken: token.s,
                region: 'southeastasia'
            }
        }),
    }, botElement);

    window.addEventListener('webchatincomingactivity', ({ data }) => {
        console.log(`Received an activity of type "${data.type}":`);
        console.log(data);
        if (data.name === 'searchFlight') {
            searchFlight(data);
        }
    });
}

const searchFlightCompleted = (activityId) => {
    botstore.dispatch({
        type: 'WEB_CHAT/SEND_EVENT',
        payload:
        {
            name: 'EVENT_DELETE_SEARCH_FLIGHT',
            value: null
        }
    });
}

init();

