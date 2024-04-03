import express from 'express';
import passport from 'passport';
import OAuth2Strategy from 'passport-oauth2';
import process from 'process';

const app = express();

class User {

    id: string;
    constructor(obj: any) {
        this.id = obj.id;
    }
    findOrCreate( f: any) {

    }
}

passport.use(new OAuth2Strategy({
    authorizationURL: 'https://identity.seasprig.dev/realms/platformservices/protocol/openid-connect/auth',
    tokenURL: 'https://identity.seasprig.dev/realms/platformservices/protocol/openid-connect/token',
    clientID: process.env.EXAMPLE_CLIENT_ID ?? '',
    clientSecret: process.env.EXAMPLE_CLIENT_SECRET ?? '',
    callbackURL: "http://localhost:3000/auth/example/callback"
  },
  function(accessToken: any, refreshToken: any, profile: any, cb: any) {
    var user = new User({ exampleId: profile.id });
    user.findOrCreate( function (err: any, user: any) {
      return cb(err, user);
    });
  }
));

app.get('/', function (req: express.Request, res: express.Response) {
    console.log(JSON.stringify(req.headers));
    res.send('hello world')
  })

var authCheck = (req: express.Request, res: express.Response) => {
    if(req.headers['Authentication']) {
        console.log('authentication found');
        res.send('hello world');
    } else {
        console.log('no authentication found');
        passport.authenticate('oauth2');
        //res.redirect('http://identity.seasprig.dev');
    }
}
app.all('*', authCheck);

app.get('/quiz/quizzes', (req: express.Request, res: express.Response) => {
    console.log('get quizzes');
    console.log(JSON.stringify(req.headers));
    if(req.headers['Authentication']) {
        console.log('authentication found');
        res.send('hello world');
    } else {
        console.log('no authentication found');
        res.status(403).json({ error: 'message' });
    }
  })

app.listen(80, () => {
  console.log(`server running on port 80`);
});