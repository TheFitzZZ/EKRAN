# EKRAN
You better watch it!

## Description

This Discord bot tries to provide an easy platform to manage and announce events. It is made for the game DCS World and will probably get features over time that don't make sense for other games. See the milestone list for features that are planned. Feed free to create issues to get help or request changes & features.

## Get the bot

To have the Bot join your Discord server simply click [here](https://discordapp.com/oauth2/authorize?client_id=396040373652291592&scope=bot).

### Donations:
If you want to support my work or just leave a little thanks, I'd appreciate a small donation towards my coffee fund :-) 

I have the following options:
- Donorbox: https://donorbox.org/dcs-cm (for PayPal and CC)
- Crypto:
  - BTC: 1KtsAMGCkJPv3R8fa2zCxznB3PhdSPDf9d
  - ETH: 0x4c336e3ea18756bef54d9d022a5788352304dbed
  - LTC: LZaWAVX35qsiYyG7VVvpwg7n9browvm3Kw
  - DASH: XxZjETznyPVwxM3z1fN315pHioFgqzZ3Gr
  - ZEC: t1eZGC319q5HSEPrDQ7Uzr88kxChbfTH1Ke
  - DOGE: DDq5ChkLt82pRYimKPRibdeWuYMQShYKdk
  - BCH: 1Ci8ZRUXzUbqjyYj2y7m5CdJAtcjF9MF4L 

Wanna get into crypto:
- [Coinbase](https://www.coinbase.com/join/5a383d1dada1050742ff705a) to buy cypto (one of the most popular methods)
- [Binance](https://www.binance.com/?ref=16671900) is my preferred exchange to trade stuff ... if you wanna dive in :-)

Cheers m8!

## Setup

After the bot joined your server, create a server role named "Calendar Admin" and add users to it that are supposed to be able to edit every entry created on that server.

## Commands for users

#### Show event calendar
**Command:** !calendar (!cal)\
**Parameters:**  none\
**Examples:** !calendar\
**Description:** Gives a compact list of upcoming events for your server.

#### Show event details 
**Command:** !event (!cev)\
**Parameters:**  eventID\
**Examples:** !event 5\
**Description:** Gives all details about the event given.

#### Attend event
**Command:** !attend \
**Parameters:** eventID \
**Examples:** !aev 3\
**Description:** Put's you on the list of attendees for the event.

#### Skip event *(aka remove your attendence)*
**Command:** !skip \
**Parameters:**  eventID \
**Examples:** !sev 3\
**Description:** Removes you from the given event.

#### Show event list
**Command:** !getevents (or !gev)\
**Parameters:** none \
**Examples:** !getevents \
**Description:** Gives a list of all upcoming events for your server.


## Commands for event creators

#### New event
**Command:** !newevent (or !nev) \
**Parameters:** "Name" "Date Time" \
**Examples:** !newevent "My Event Name" "2018-12-24 13:37"\
**Description:** Creates a new event with the given name and date+time. If the name has more than one word, it needs to be put in quotation marks. Date and time is entered in the format "YYYY-MM-DD HH:MM" in the 24h format with quotation marks. There is no timezone conversion (yet), so use UTC or the favorite TZ of your server. 

After you create an event, the bot will tell you the assigned ID and send you a DM explaining how you can edit & enhance it with more information.

#### Edit event
**Command:** !editevent (or !eev)\
**Parameters:** several :-) 
- Add a max attendee limit: !editev *eventid* **maxattendees** 15
- Add a URL to a briefing: !editev *eventid* **briefingurl** http://someurl.com/briefing
- Add a URL to a Discord invite: !editev *eventid* **discordurl** http://discord.gg/yourinviteid
- Add a short description to the event: !editev *eventid* **description** "Description goes here"
- Toggle public flag of the event: !editev *eventid* **public**" 
- Edit the event name: !editev *eventid* **name** "My Eventname"
- Edit the event date: !editev *eventid* **date** "2018-12-24 13:37" 

To state the obvious: replace *eventid* with the actual id!

**Examples:** See list above. \
**Description:** Allows you to edit several information on the event with the given ID. Only the creator of the event can edit it.

#### Delete event
**Command:** !deleteevent (or !dev) \
**Parameters:** eventID \
**Examples:** !dev 3 \
**Description:** Deletes the event with the given ID. You can only delete an event that you created.

