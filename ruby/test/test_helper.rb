# frozen_string_literal: true

require 'minitest/autorun'
require 'tmpdir'
require_relative '../book'
require_relative '../catalog'
require_relative '../storage'
require_relative '../validation'

# Builds a small catalog used by several tests.
def sample_catalog
  Catalog.new([
                Book.new(id: 1, title: 'The Hobbit', author: 'J.R.R. Tolkien', genre: 'Fantasy', year: 1937),
                Book.new(id: 2, title: '1984', author: 'George Orwell', genre: 'Dystopian', year: 1949),
                Book.new(id: 3, title: 'Animal Farm', author: 'George Orwell', genre: 'Political Satire', year: 1945),
                Book.new(id: 4, title: 'Dune', author: 'Frank Herbert', genre: 'Science Fiction', year: 1965),
                Book.new(id: 5, title: 'The Fellowship of the Ring', author: 'J.R.R. Tolkien', genre: 'fantasy', year: 1954)
              ])
end
