# Warthog

## Description

This Discord bot tries to provide an easy platform to manage and announce events. It is made for the game DCS World and will probably get features over time that don't make sense for other games. See the milestone list for features that are planned. Feed free to create issues to get help or request changes & features.


## Donations

If you and/or your community gets use out of it, any donations are super welcome. It'll pay for the coffee destroyed in the process writing this :-)

ETH:
LTC:
DOGE:

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

