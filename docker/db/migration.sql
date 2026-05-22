-- マイグレーション: master_itemsテーブルにmemoカラムを追加
ALTER TABLE master_items ADD COLUMN memo TEXT;
