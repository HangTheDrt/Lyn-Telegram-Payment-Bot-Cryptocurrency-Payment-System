using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace telegram_Payment
{
    internal class database
    {
        import sqlite3

class SQLiteDBManager :
    def __init__(self, db_path):
        self.db_path = db_path
        self._create_table()

    def _connect(self):
        return sqlite3.connect(self.db_path)

    def _create_table(self) :
        conn = self._connect()
        cursor = conn.cursor()
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS orders(
                uniqid TEXT PRIMARY KEY,
                chat_id INTEGER,
                user_id INTEGER,
                username TEXT,
                status TEXT,
                crypto TEXT,
                amount INTEGER,
                plan TEXT,
                hash TEXT
            )
        ''')
        conn.commit()
        conn.close()

    def insert_order(self, chat_id, user_id, username, uniqid, status, crypto, usdvalue, selected_plan, hash):
        conn = self._connect()
        cursor = conn.cursor()
        cursor.execute('''
            INSERT INTO orders(chat_id, user_id, username, uniqid, status, crypto, amount, plan, hash)
            VALUES(?, ?, ?, ?, ?, ?, ?, ? ,?)
        ''', (chat_id, user_id, username, uniqid, status, crypto,usdvalue,selected_plan, hash))
        conn.commit()
        conn.close()

    def update_order_status(self, uniqid, new_status):
        conn = self._connect()
        cursor = conn.cursor()
        cursor.execute('''
            UPDATE orders SET status = ? WHERE uniqid = ?
        ''', (new_status, uniqid))
        conn.commit()
        conn.close()

    def get_order_status(self, uniqid):
        conn = self._connect()
        cursor = conn.cursor()
        cursor.execute('SELECT status FROM orders WHERE uniqid = ?', (uniqid,))
        status = cursor.fetchone()
        conn.close()
        return status[0] if status else None

    def get_order_details(self, uniqid) :
        conn = self._connect()
        cursor = conn.cursor()
        cursor.execute('SELECT * FROM orders WHERE uniqid = ?', (uniqid,))
        details = cursor.fetchone()
        conn.close()
        return details

    def update_order_hash(self, uniqid, crypto_hash) :
        conn = self._connect()
        cursor = conn.cursor()
        cursor.execute('''
            UPDATE orders SET hash = ? WHERE uniqid = ?
        ''', (crypto_hash, uniqid))
        conn.commit()
        conn.close()

    }
}
