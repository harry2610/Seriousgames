import http.server
import json
from urllib.parse import urlparse, parse_qs

# Stores the leaderboard in leaderboard.json in the current directory
#
# Use "python3 leaderboardserver.py" to run the server
# Use "tailscale funnel 8075" to expose the server to the internet
# Use "curl -X POST -d '{"player": "Alice", "dog": "Spot", "score": 100}' http://localhost:8075/leaderboard" to add a score
# Use "curl http://localhost:8075/leaderboard" to get the leaderboard

leaderboard = []

def save_leaderboard(leaderboard):
    try:
        with open('leaderboard.json', 'w') as f:
            json.dump(leaderboard, f)
    except Exception as e:
        print(f"Error saving leaderboard: {e}")
        
def load_leaderboard():
    try:
        with open('leaderboard.json', 'r') as f:
            return json.load(f)
    except FileNotFoundError:
        print("Leaderboard file not found")
        return []
    except Exception as e:
        print(f"Error loading leaderboard: {e}")
        return []

class LeaderboardHandler(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == '/leaderboard':
            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            leaderboard.sort(key=lambda x: x['score'], reverse=True)
            self.wfile.write(json.dumps(leaderboard).encode())
        else:
            self.send_response(404)
            self.end_headers()

    def do_POST(self):
        if self.path == '/leaderboard':
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length)
            entry = json.loads(post_data)
            
            # Validate entry
            if 'player' in entry and 'dog' in entry and 'score' in entry:
                for existing_entry in leaderboard:
                    if existing_entry['player'] == entry['player'] and existing_entry['dog'] == entry['dog']:
                        existing_entry['score'] = entry['score']
                        break
                else:
                    leaderboard.append(entry)
                
                save_leaderboard(leaderboard)
                self.send_response(201)
                self.end_headers()
            else:
                self.send_response(400)
                self.end_headers()
        else:
            self.send_response(404)
            self.end_headers()

def run(server_class=http.server.HTTPServer, handler_class=LeaderboardHandler, port=8075):
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    global leaderboard
    leaderboard = load_leaderboard()
    print(f'Starting httpd server on port {port}')
    httpd.serve_forever()

if __name__ == "__main__":
    run()